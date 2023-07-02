using AlterbladeBot.GameLib.Modes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.GameLib.GameObjects.Statuses
{
	internal class BattleStatus : Status
	{

		readonly BattleStatusType battleStatusType;
		readonly Battle battle;

		public BattleStatusType BattleStatusType => battleStatusType;

		public BattleStatus(string name, int duration, Battle battle, Hero source, Skill sourceSkill, BattleStatusType battleStatusType)
			: base(name, duration, source, sourceSkill)
		{
			this.battleStatusType = battleStatusType;
			this.battle = battle;
		}

		public override bool End(bool showText)
		{
			StringBuilder output = new StringBuilder();
			switch (battleStatusType)
			{
				case BattleStatusType.TRICKROOM:
				{
					battle.HeroQueueSort = HeroQueueSort.SPEED;
					output.Append("The twisting dimensions turned to normal.");
					break;
				}
				case BattleStatusType.SCORCH:
				{
					output.Append("The raging [cyan]Scorch[/cyan] subsided.");
					break;
				}
			}
			if (showText)
				Utils.DelayedWrite(output, 1000);

			return battle.RemoveBattleStatus(this);
		}

		public override bool Update()
		{
			if (duration-- == 0)
			{
				End(true);
				return false;
			}
			switch(battleStatusType)
			{
				case BattleStatusType.TRICKROOM:
				{
					Utils.DelayedWrite("The dimensions are twisting disorderly.", 1000);
					break;
				}
				case BattleStatusType.SCORCH:
				{
					Utils.DelayedWrite("The [cyan]Scorch[/cyan] is engulfing the battlefield!", 1000);
					for (int i = 0; i < battle.HeroQueue.Count; i++)
					{
						Hero hero = battle.HeroQueue[i];
						Debug.WriteLine(hero.Statuses.Count);
						if (hero.Statuses.Count > 0)
						{
							float percentDamage = 0.1F * hero.Statuses.Count;
							Utils.DelayedWrite(new StringBuilder().AppendFormat("{0} was afflicted with [cyan]Scorch[/cyan]!", hero.Name), 1000);
							hero.TakeDamage(percentDamage, false);
						}
					}
					break;
				}
			}
			return true;
		}
	}
}
