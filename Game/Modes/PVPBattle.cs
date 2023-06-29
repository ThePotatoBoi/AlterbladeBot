using Alterblade;
using AlterbladeBot.Game.GameObjects;
using DSharpPlus;
using DSharpPlus.Entities;
using PrototypeA.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.Game.Modes
{
    internal class PVPBattle
    {

        public PVPBattleType BattleType { private set; get; }
		public DiscordChannel Channel { private set; get; }
		public List<Player> Players { private set; get; }

        public PVPBattle(PVPBattleType battleType, DiscordUser userA, DiscordUser userB, DiscordChannel channel)
		{
			BattleType = battleType;
			Channel = channel;

			Players = new List<Player>
			{
				new Player(userA),
				new Player(userB)
			};
 		}

		public static Hero HeroSelector(Player player)
		{
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
				.WithTitle($"Select a hero, {player.User.Username}!");

			for (int i = 0; i < GameConstants.HEROES.Count; i++)
			{
				Hero hero = GameConstants.HEROES[i];
			}

			return GameConstants.HEROES[i];
		}

	}
}
