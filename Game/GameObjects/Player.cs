using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.Game.GameObjects
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

		public bool AddHeroToTeam(Hero hero)
		{
			if ( Team.Count > 2 )
			{
				Team.Add(hero);
				return true;
			}
			return false;
		}
    }
}
