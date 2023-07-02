using AlterbladeBot.BotObjects;
using AlterbladeBot.GameLib.GameObjects;
using AlterbladeBot.GameLib;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.BotHelpers
{
	internal static class GameEmbedMaker
	{
		public static DiscordEmbedBuilder HeroListEmbed()
		{
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
				.WithTitle("Hero Selection")
				.WithColor(DiscordColor.DarkButNotBlack)
				.WithDescription("Use `/selectHeroes` to choose.");
			for (int i = 0; i < GameConstants.HEROES.Count; i++)
			{
				Hero hero = GameConstants.HEROES[i];
				StringBuilder fieldValue = new StringBuilder().Append("```ansi\r\n\u001b[0;37;45m  STATS  \u001b[0m\r");
				foreach (KeyValuePair<Stats, int> item in hero.BaseStats)
				{
					if ( item.Key == Stats.CRIT_CHANCE ) { continue; }
					fieldValue.AppendFormat(
						"\u001b[0;34m{0,-7}\u001b[0m : {1}\r",
						item.Key.ToString(),
						item.Value
					);
				}
				fieldValue.Append("\n\u001b[0;37;45m  MOVES  \u001b[0m\r");
				for (int j = 0; j < hero.Skills.Count; j++)
				{
					Skill skill = hero.Skills.ElementAt(j);
					fieldValue.AppendFormat(
						"\u001b[0;{1}m{0,-7}\u001b[0m\r",
						skill.Name,
						j >= hero.Skills.Count - 1 ? 31 : 33
					);
				}
				embedBuilder.AddField(hero.Name, fieldValue.Append("```").ToString(), true);
			}
			return embedBuilder;
		}
		public static DiscordEmbedBuilder ErrorEmbed(string title, string description)
		{
			return new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Red)
				.WithTitle(title)
				.WithDescription(description);
		}
		public static DiscordEmbedBuilder GameStatusEmbed(Game game)
		{
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

			return embedBuilder;
		}
	}
}
