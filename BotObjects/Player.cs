using AlterbladeBot.GameLib.GameObjects;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.BotObjects
{
	internal class Player
	{
		public DiscordUser User { private set; get; }
		public List<Hero> Team { private set; get; }
		public Player(DiscordUser user)
		{
			User = user;
			Team = new List<Hero>();
		}
	}
}
