using System;
using System.Text;

namespace AlterbladeBot.GameLib.GameObjects.Statuses
{
	internal class HeroStatus : Status
	{

		readonly Hero hero;
		readonly StatusType statusType;
		readonly bool isNegative;

		public Hero Hero => hero;
		public bool IsNegative => isNegative;

		public HeroStatus(string name, int duration, Hero hero, Hero source, Skill sourceSkill, StatusType statusType)
			: base(name, duration, source, sourceSkill)
		{
			this.hero = hero;
			this.statusType = statusType;
			isNegative = statusType.ToString()[0] == 'N';
		}

		public override bool End(bool showText)
		{
			StringBuilder output = new StringBuilder();
			switch (statusType)
			{
				case StatusType.N_DAMAGE_PER_TURN:
				{
					output.AppendFormat("{0} is free from [cyan]{1}[/cyan]!", hero.Name, name);
					break;
				}
				case StatusType.N_DEATH_NOTICE:
				{
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] ended.", hero.Name, name);
					break;
				}
				case StatusType.N_DISABLE:
				{
					for (int i = 0; i < hero.Skills.Count; i++)
					{
						if (hero.Skills[i].IsDisabled)
							hero.Skills[i].IsDisabled = false;
					}
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] ended.", hero.Name, name);
					break;
				}
				case StatusType.P_CRITBOOST:
				{
					hero.CurrentStats[Stats.CRIT_CHANCE] -= 20;
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] petered out ended.", hero.Name, name);
					break;
				}
				case StatusType.N_TAUNT:
				{
					hero.PriorityTarget = Hero.None;
					output.AppendFormat("{0}'s [cyan]{1}[/cyan] ended.", hero.Name, name);
					break;
				}
				case StatusType.N_FEEBLE:
				{
					hero.IsFeeble = false;
					output.AppendFormat("{0} was freed from [cyan]{1}[/cyan].", hero.Name, name);
					break;
				}
			}
			if (showText)
				Utils.DelayedWrite(output, 1000);
			
			return hero.RemoveStatus(this); ;
		}

		public override bool Update()
		{
			if (duration-- == 0)
			{
				End(true);
				return false;
			}

			StringBuilder output = new StringBuilder();

			switch (statusType)
			{
				case StatusType.N_DAMAGE_PER_TURN:
				{
					output.AppendFormat("{0} is hurt by [cyan]{1}[/cyan]!", hero.Name, name);
					Utils.DelayedWrite(output, 1000);
					hero.TakeDamage(0.08F, false);
					break;
				}
				case StatusType.N_DEATH_NOTICE:
				{
					if (Hero.CurrentStats[Stats.HP] < Convert.ToInt32(0.2F * hero.BaseStats[Stats.HP]))
					{
						output.AppendFormat("{0} is caught by the [cyan]{1}[/cyan]!", hero.Name, name);
						Utils.DelayedWrite(output, 1000);
						hero.RemoveStatus(this);
						hero.TakeDamage(1F, false);
					}
					else
					{
						output.AppendFormat("{0}'s [cyan]{1}[/cyan] is pending...", hero.Name, name);
						Utils.DelayedWrite(output, 1000);
					}
					break;
				}

			}
			return true;
		}
	}
}
