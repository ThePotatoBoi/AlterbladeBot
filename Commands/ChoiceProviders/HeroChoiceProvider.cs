using Alterblade;
using Alterblade.GameObjects;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Bot.Commands.ChoiceProviders
{
	internal class HeroChoiceProvider : IChoiceProvider
	{
		public async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
		{
			DiscordApplicationCommandOptionChoice[] heroChoices = new DiscordApplicationCommandOptionChoice[GameConstants.HEROES.Count];
			for (int i = 0; i < GameConstants.HEROES.Count; i++)
			{
				Hero hero = GameConstants.HEROES.ElementAt(i);
				heroChoices[i] = new DiscordApplicationCommandOptionChoice(hero.Name, hero.Name);
			}
			return heroChoices;
		}
	}
}
