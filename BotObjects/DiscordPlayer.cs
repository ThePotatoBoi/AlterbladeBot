using Alterblade.GameObjects;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.BotObjects
{
	internal class DiscordPlayer
	{
		public DiscordUser User { private set; get; }
		public List<Hero> Team { private set; get; }
		public DiscordPlayer(DiscordUser user)
		{
			User = user;
			Team = new List<Hero>();
		}
	}
}
