using AlterbladeBot.GameLib.GameObjects.Statuses;
using AlterbladeBot.GameLib.Modes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlterbladeBot.GameLib.GameObjects
{
	internal class Hero
	{

		#region Constants

		readonly static Hero none = new Hero("Unknown", "???", new Dictionary<Stats, int>(), new List<Skill>(), null);
		public static Hero None => none;

		#endregion

		#region Fields

		readonly string name;
		readonly string title;

		readonly List<Skill> skills;
		readonly List<Hero> team;
		readonly Dictionary<Stats, int> baseStats = new Dictionary<Stats, int>();
		readonly Dictionary<Stats, int> currentStats = new Dictionary<Stats, int>();
		readonly Dictionary<Stats, int> statModifiers = new Dictionary<Stats, int>() { { Stats.ATTACK, 0 }, { Stats.DEFENSE, 0 }, { Stats.SPEED, 0 } };
		readonly List<HeroStatus> statuses = new List<HeroStatus>();

		bool isAlive = true;
		bool isFeeble = false;
		bool isSupressed = false;

		#endregion

		#region Properties
		public string Name => name;
		public string Title => title;
		public List<Skill> Skills => skills;
		public Dictionary<Stats, int> BaseStats => baseStats;
		public Dictionary<Stats, int> CurrentStats => currentStats;
		public List<Hero> Team => team;
		public List<HeroStatus> Statuses => statuses;
		public Skill LastSkillUsed { get; set; }
		public Skill LastSkillHit { get; set; }
		public Hero PriorityTarget { get; set; }
		public Hero LastHeroAttacker { get; set; }
		public bool IsAlive => isAlive;
		public bool IsSupressed => isSupressed;
		public bool IsFeeble { get { return isFeeble; } set { isFeeble = value; UpdateStats(); } }
		public float HealingScale { get; set; } = 1F;
		public string StatisticsBanner
		{
			get
			{
				StringBuilder output = new StringBuilder();
				output.AppendFormat("{0} the {1}\n", name, title);
				output.AppendFormat("  █ HP  {0, 4}     █ {1}\n", currentStats[Stats.HP], skills[0].Name);
				output.AppendFormat("  █ ATK {0, 4}     █ {1}\n", currentStats[Stats.ATTACK], skills[1].Name);
				output.AppendFormat("  █ DEF {0, 4}     █ {1}\n", currentStats[Stats.DEFENSE], skills[2].Name);
				output.AppendFormat("  █ SPE {0, 4}     █ {1}\n", currentStats[Stats.SPEED], skills[3].Name);
				return output.ToString();
			}
		}
		public string InBattleStatisticsBanner
		{
			get
			{
				StringBuilder output = new StringBuilder();

				StringBuilder greenBar = new StringBuilder();
				StringBuilder magentaBar = new StringBuilder();

				int spaceCount = (int)Math.Ceiling(20F * currentStats[Stats.HP] / baseStats[Stats.HP]);
				for (int i = 0; i < spaceCount; i++)
					greenBar.Append(' ');
				for (int i = 0; i < 20 - spaceCount; i++)
					magentaBar.Append(' ');
#if DEBUG
				output.AppendFormat("{0}\t [green]~{1}[/green][magenta]~{2}[/magenta] {3}/{4}\n", name, greenBar, magentaBar, currentStats[Stats.HP], baseStats[Stats.HP]);
#else
				output.AppendFormat("{0}\t [green]~{1}[/green][magenta]~{2}[/magenta] {3}\n", name, greenBar, magentaBar, currentStats[Stats.HP]);
#endif
				for (int i = 0; i < statuses.Count; i++)
				{
					if (i == 0) { output.Append("    █ "); }
					string color = statuses[i].IsNegative ? "red" : "green";
					output.AppendFormat("[{2}]{0}({1})[/{2}]", statuses[i].Name, statuses[i].Duration, color);
					if (i != statuses.Count - 1)
						output.Append(", ");
					else
						output.Append('\n');
				}
#if DEBUG
				output.AppendFormat(
					"    █ ATK {0, -7} SPE {1}\n",
					new StringBuilder().AppendFormat("{0, 3}/{1, -3}", currentStats[Stats.ATTACK], baseStats[Stats.ATTACK]).ToString().Trim(),
					new StringBuilder().AppendFormat("{0, 3}/{1, -3}", currentStats[Stats.SPEED], baseStats[Stats.SPEED]).ToString().Trim()
				);
				output.AppendFormat(
					"    █ DEF {0, -7} CRI {1}%",
					new StringBuilder().AppendFormat("{0, 3}/{1, -3}", currentStats[Stats.DEFENSE], baseStats[Stats.DEFENSE]).ToString().Trim(),
					currentStats[Stats.CRIT_CHANCE]
				);
#else
				output.AppendFormat(
					"    █ ATK {0, -6} SPE {1}\n",
					currentStats[Stats.ATTACK],
					currentStats[Stats.SPEED]
				);
				output.AppendFormat(
					"    █ DEF {0, -6} CRI {1}%",
					currentStats[Stats.DEFENSE],
					currentStats[Stats.CRIT_CHANCE]
				);
#endif
				return output.ToString();
			}
		}
		public string InBattleSkillsBanner
		{
			get
			{
				StringBuilder output = new StringBuilder();
				for (int i = 0; i < skills.Count; i++)
				{
					output.AppendFormat(
						"  [cyan]{0, -20}[/cyan] | {1, -3} | {2, -4}| {3}\n",
						skills[i].Name,
						skills[i].BaseDamage < 1 ? "-" : skills[i].BaseDamage,
						skills[i].Accuracy <= 0 ? '-' : new StringBuilder().Append(Convert.ToInt32(skills[i].Accuracy * 100)).Append('%'),
						skills[i].SkillPoint
					);
				}
				return output.ToString();
			}
		}

		#endregion

		#region Constructor

		public Hero(string name, string title, Dictionary<Stats, int> stats, List<Skill> skills, List<Hero>? team)
		{
			this.name = name;
			this.title = title;
			baseStats = new Dictionary<Stats, int>(stats);
			currentStats = new Dictionary<Stats, int>(stats);

			// Cloning skills
			this.skills = new List<Skill>();
			for (int i = 0; i < skills.Count; i++)
				this.skills.Add(new Skill(skills[i]));

			this.team = team is null ? new List<Hero>() : team;

			LastHeroAttacker = none;
			PriorityTarget = none;
			LastSkillHit = Skill.None;
			LastSkillUsed = Skill.None;
		}

		#endregion

		void CheckAliveCondition()
		{
			if (currentStats[Stats.HP] < 1)
			{
				isAlive = false;
				team.Remove(this);
			}
		}

		public void DoSkill(Battle battle, Skill? skill = null)
		{
			if (skill is null)
				Utils.WriteLineColor(InBattleSkillsBanner);
			while (true)
			{
				if (skill is null)
				{
					int input = Utils.GetInteger(1, 4, new StringBuilder().AppendFormat("{0}: ", name).ToString());
					skill = skills[input - 1];
				}
				if (skill.Activate(this, battle)) { break; } else { skill = null; }
			}
			LastSkillUsed = skill;
		}

		public bool RollCrit(float bonusCrit = 0F)
		{
			return Utils.RollBoolean((currentStats[Stats.CRIT_CHANCE] * 0.01F) + bonusCrit);
		}

		#region Damage Handlers

		/// <summary>
		/// Method <c>TakeDamage</c> decreases hero's HP, 
		/// </summary>
		/// <param name="amount">This defines the true amount of HP decrease.</param>
		/// <param name="showText">If set to <c>true</c>, this method will show damage taken text.</param>
		/// <param name="isCrit">If set to <c>true</c>, <c>amount</c> is multiplied by 1.5F.</param>
		/// <param name="isTrueDamage">If set to <c>true</c>, damage staple is multiplied by a random 0.9F to 1.0F floating point number.</param>
		/// <returns>Returns an <c>int</c> containing the damage taken of the <c>Hero</c> object or a <c>-1</c> if the <c>amount<0</c> or the <c>Hero</c> object taking damage is not alive..</returns>
		public int TakeDamage(int amount, bool isCrit, bool isTrueDamage, bool showText)
		{
			if (!isAlive || amount < 0) { return -1; }

			if (!isTrueDamage)
			{
				float multiplier = isCrit ? 1.5F : 1F;
				float bonusMultiplier = 0.9F + Convert.ToSingle(0.2F * Utils.Random.NextDouble());
				amount = Convert.ToInt32( amount * multiplier * bonusMultiplier );
			}

			amount = Math.Clamp(amount, 0, currentStats[Stats.HP]);
			currentStats[Stats.HP] = Math.Clamp(currentStats[Stats.HP] - amount, 0, baseStats[Stats.HP]);
			if (showText)
			{
				StringBuilder output = new StringBuilder();
				output.AppendFormat("{0} takes {1} damage! {2}", name, amount, isCrit ? "[red]It's a critical hit![/red]" : "").ToString();
				Utils.DelayedWrite(output.ToString(), 1000);
			}
			CheckAliveCondition();
			return amount;
		}

		public int TakeDamage(int baseDamage, int attackerAttack, bool showText, bool isCrit, bool ignoreDefense)
		{
			if (isCrit) { ignoreDefense = true; }
			int damage = Utils.CalculateDamage(baseDamage, ignoreDefense, attackerAttack, currentStats[Stats.DEFENSE], baseStats[Stats.DEFENSE]);
			return TakeDamage(damage, isCrit, false, showText);
		}

		public int TakeDamage(float percent, bool showText)
		{
			return TakeDamage(Convert.ToInt32(baseStats[Stats.HP] * percent), false, true, showText);
		}

		public int Heal(int amount, bool showText)
		{
			if (amount < 1) { return -1; }
			int temp = currentStats[Stats.HP];
			currentStats[Stats.HP] = Math.Clamp(currentStats[Stats.HP] + amount, 0, baseStats[Stats.HP]);
			if (showText)
			{
				StringBuilder output = new StringBuilder().AppendFormat("{0} regained [yellow]{1}[/yellow] HP!", name, amount);
				Utils.DelayedWrite(output.ToString(), 1000);
			}
			CheckAliveCondition();
			return currentStats[Stats.HP] - temp;
		}

		public void Heal(float percent, bool isScaleWithMissingHP, bool showText)
		{
			int staple = isScaleWithMissingHP
				? baseStats[Stats.HP] - currentStats[Stats.HP]
				: baseStats[Stats.HP];
			Heal( Convert.ToInt32(percent * staple), showText );
		}

		#endregion

		#region Stat Handlers

		public bool ModifyStats(Stats stat, int amount, bool showText = true)
		{
			if (amount == 0) { return false; }
			if (stat == Stats.RANDOM) { stat = (Stats) Utils.Random.Next(1, 4); }

			if (Convert.ToInt32(stat) < 1 || Convert.ToInt32(stat) > 3)
			{
				Utils.Error("Such STAT cannot be modified.");
				return false;
			}

			amount = Math.Clamp(amount, -3, 3);
			int temp = statModifiers[stat];
			statModifiers[stat] = Math.Clamp(statModifiers[stat] + amount, -3, 6);
			int delta = statModifiers[stat] - temp;
			UpdateStats();

			string prefix = amount < 0 ? "fell" : "rose";
			string adverb = "";
			if (Math.Abs(delta) > 1) adverb = Math.Abs(delta) == 2 ? "sharply " : "drastically ";

			StringBuilder output = new StringBuilder();
			if (delta == 0)
			{
				if (showText)
				{
					prefix = amount < 0 ? "lowered" : "raised";
					output.AppendFormat("{0}'s {1} cannot be {2} anymore!", name, stat.ToString(), prefix);
					Utils.DelayedError(output.ToString(), 1000);
				}
				return false;
			}
			else
			{
				if (showText)
				{
					output.AppendFormat("{0}'s {1} {2}{3}!", name, stat.ToString(), adverb, prefix);
					Utils.DelayedWrite(output.ToString(), 1000);
				}
				return true;
			}
			
		}

		void UpdateStats()
		{
			for (int i = 1; i < 4; i++)
			{
				currentStats[(Stats) i] = baseStats[(Stats) i];
				int delta = Convert.ToInt32(baseStats[(Stats) i] * 0.25F);
				currentStats[(Stats) i] = currentStats[(Stats)i] + (statModifiers[(Stats) i] * delta);
				if (i == 1 && IsFeeble)
					currentStats[(Stats)i] = Convert.ToInt32(currentStats[(Stats)i] * 0.5F);
			}
		}

		#endregion

		#region Status Handlers

		public void UpdateStatuses()
		{
			for (int i = 0; i < statuses.Count; i++)
			{
				statuses[i].Update();
			}
		}

		public bool AddStatus(HeroStatus status, bool showText)
		{
			for (int i = 0; i < statuses.Count; i++)
			{
				if (status.Name == statuses[i].Name)
				{
					Utils.Error("Status already exists!");
					return false;
				}
			}
			statuses.Add(status);
			return true;
		}

		public bool RemoveStatus(HeroStatus status)
		{
			return statuses.Remove(status);
		}

		#endregion

	}
}