using AlterbladeBot.GameLib.GameObjects;
using AlterbladeBot.GameLib.GameObjects.Statuses;
using DSharpPlus.SlashCommands;
using System.Collections.Generic;

namespace AlterbladeBot.GameLib
{
	enum Stats
	{
		HP,
		ATTACK,
		DEFENSE,
		SPEED,
		CRIT_CHANCE,
		RANDOM
	}

	enum StatusType
	{
		N_TAUNT,
		N_DAMAGE_PER_TURN,
		N_DEATH_NOTICE,
		N_DISABLE,
		P_CRITBOOST,
		N_FEEBLE,
	}

	enum BattleStatusType
	{
		TRICKROOM,
		SCORCH
	}

	enum SkillTarget
	{
		TARGET,
		NONE,
		ENEMYTEAM,
		ALLYTEAM,
	}

	enum SkillAction
	{
		// General
		DAMAGE,
		DAMAGE_HEAL,
		DAMAGE_RECOIL,
		DAMAGE_IGNORE_TARGET_DEFENSE,
		DAMAGE_SCALE_WITH_TARGET_ATTACK,
		SELF_STAT,
		ENEMY_STAT,
		SELF_STAT_CHANCE,
		ENEMY_STAT_CHANCE,
		HEAL_PERCENT,
		HEAL_MISSING,
		DOUBLEHITS,
		TRIPLEHITS,
		MULTIHITS,

		// Specials
		TAUNT,
		MIMIC,

		// Status
		THORNS,
		DISABLE,
		DEATH_NOTICE,
		POISON_CHANCE,
		CRITBOOST,
		FEEBLE,

		// Battle Status
		TRICKROOM,
		SCORCH
	}

	enum HeroQueueSort
	{
		SPEED,
		SPEED_REVERSED,
	}

	enum PVPBattleType
	{
		[ChoiceName("Hero vs Hero")]
		NORMAL,
		[ChoiceName("Team vs Team")]
		TEAM
	}

	internal static class GameConstants
	{
		public static List<Hero> HEROES = new List<Hero>
		{

			new Hero(
				"Effelia",
				"Druidess",
				new Dictionary<Stats, int>()
				{
					{ Stats.HP, 512 },
					{ Stats.ATTACK, 90 },
					{ Stats.DEFENSE, 85 },
					{ Stats.SPEED, 85 },
					{ Stats.CRIT_CHANCE, 8 }
				},
				new List<Skill>()
				{
					new Skill( "Pierce", 70, 25, 0.95F,
						new List<SkillAction>() { SkillAction.DAMAGE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Nature's Blessing", 0, 10, 0,
						new List<SkillAction>() { SkillAction.SELF_STAT }, SkillTarget.NONE,
						new Dictionary<Stats, int> { { Stats.ATTACK, 1 }, { Stats.SPEED, 1 } }, false ),
					new Skill( "Garden of Thorns", 0, 5, 0.9F,
						new List<SkillAction>() { SkillAction.THORNS }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Siphon Life", 100, 3, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE_HEAL }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, true )
				},
				null
			),

			new Hero(
				"Axel",
				"Paladin",
				new Dictionary<Stats, int>()
				{
					{ Stats.HP, 568 },
					{ Stats.ATTACK, 75 },
					{ Stats.DEFENSE, 110 },
					{ Stats.SPEED, 55 },
					{ Stats.CRIT_CHANCE, 6 }
				},
				new List<Skill>()
				{
					new Skill( "Aura Blade", 70, 15, 0.95F,
						new List<SkillAction>() { SkillAction.DAMAGE_IGNORE_TARGET_DEFENSE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Shield Bash", 50, 10, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT_CHANCE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { { Stats.DEFENSE, -1 } }, false ),
					new Skill( "Taunt", 0, 5, 0,
						new List<SkillAction>() { SkillAction.TAUNT }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Holy Blessing", -25, 1, 0,
						new List<SkillAction>() { SkillAction.HEAL_PERCENT, SkillAction.SELF_STAT }, SkillTarget.ALLYTEAM,
						new Dictionary<Stats, int>() { { Stats.DEFENSE, 1 } }, true )
				},
				null
			),

			new Hero(
				"Cadeceus",
				"Plaque Doctor",
				new Dictionary<Stats, int>()
				{
					{ Stats.HP, 545 },
					{ Stats.ATTACK, 106 },
					{ Stats.DEFENSE, 78 },
					{ Stats.SPEED, 78 },
					{ Stats.CRIT_CHANCE, 8 }
				},
				new List<Skill>()
				{
					new Skill( "Incision", 70, 25, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Noxious Blast", 40, 10, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT }, SkillTarget.TARGET,
						new Dictionary<Stats, int> { { Stats.SPEED, -1 } }, false ),
					new Skill( "Death Notice", 0, 3, 0,
						new List<SkillAction>() { SkillAction.DEATH_NOTICE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Corrosive Rain", 120, 1, 0.95F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT }, SkillTarget.ENEMYTEAM,
						new Dictionary<Stats, int> { { Stats.RANDOM, -1 } }, true )
				},
				null
			),

			new Hero(
				"Huabbi",
				"Two-faced Djinn",
				new Dictionary<Stats, int>()
				{
					{ Stats.HP, 590 },
					{ Stats.ATTACK, 70 },
					{ Stats.DEFENSE, 75 },
					{ Stats.SPEED, 65 },
					{ Stats.CRIT_CHANCE, 6 }
				},
				new List<Skill>()
				{
					new Skill( "Dual Orb", 50, 15, 0.90F,
						new List<SkillAction>() { SkillAction.DOUBLEHITS }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Mimic", 0, 10, 0,
						new List<SkillAction>() { SkillAction.MIMIC }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { { Stats.DEFENSE, -1 } }, false ),
					new Skill( "Trick Room", 0, 3, 0,
						new List<SkillAction>() { SkillAction.TRICKROOM }, SkillTarget.NONE,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Distortion", 90, 5, 0,
						new List<SkillAction>() { SkillAction.DAMAGE_SCALE_WITH_TARGET_ATTACK }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, true )
				},
				null
			),

			new Hero(
				"Medea",
				"Shadow Elf",
				new Dictionary<Stats, int>()
				{
					{ Stats.HP, 490 },
					{ Stats.ATTACK, 115 },
					{ Stats.DEFENSE, 70 },
					{ Stats.SPEED, 96 },
					{ Stats.CRIT_CHANCE, 10 }
				},
				new List<Skill>()
				{
					new Skill( "Cripping Shot", 70, 15, 0.9F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.ENEMY_STAT_CHANCE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { { Stats.ATTACK, -1 } }, false ),
					new Skill( "Bowfaire", 0, 5, 0,
						new List<SkillAction>() { SkillAction.CRITBOOST }, SkillTarget.NONE,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Disable", 0, 5, 0,
						new List<SkillAction>() { SkillAction.DISABLE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Arrow Barrage", 30, 3, 0.9F,
						new List<SkillAction>() { SkillAction.MULTIHITS }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, true )
				},
				null
			),

			new Hero(
				"Volfir",
				"Dark Magician",
				new Dictionary<Stats, int>()
				{
					{ Stats.HP, 455 },
					{ Stats.ATTACK, 126 },
					{ Stats.DEFENSE, 72 },
					{ Stats.SPEED, 92 },
					{ Stats.CRIT_CHANCE, 6 }
				},
				new List<Skill>()
				{
					new Skill( "Swarm", 30, 15, 0.90F,
						new List<SkillAction>() { SkillAction.TRIPLEHITS }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Enfeeble", 20, 10, 0.8F,
						new List<SkillAction>() { SkillAction.DAMAGE, SkillAction.FEEBLE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Scorch", 0, 5, 0,
						new List<SkillAction>() { SkillAction.SCORCH }, SkillTarget.NONE,
						new Dictionary<Stats, int>() { }, false ),
					new Skill( "Impending Doom", 160, 1, 0,
						new List<SkillAction>() { SkillAction.DAMAGE_IGNORE_TARGET_DEFENSE }, SkillTarget.TARGET,
						new Dictionary<Stats, int>() { }, true )
				},
				null
			)
		};
		public static Hero? GetHeroFromName(string name)
		{
			for (int i = 0; i < HEROES.Count; i++)
			{
				if (HEROES[i].Name.ToLower() == name.ToLower())
				{
					return HEROES[i];
				}
			}
			return null;
		}
	}
}