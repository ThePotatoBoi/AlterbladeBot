using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Bot.BotObjects;

namespace Bot
{
	enum PVPBattleType
	{
		[ChoiceName("Hero vs Hero")]
		NORMAL,
		[ChoiceName("Team vs Team")]
		TEAM
	}

	internal class Bot
	{
		public DiscordClient Client { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public SlashCommandsExtension SlashCommands { get; private set; }
		public Config Config { get; private set; }

		private static Dictionary<ulong, Game> gameInstances = new Dictionary<ulong, Game>(); 

		public async Task RunAsync()
		{
			using ( FileStream fs = File.OpenRead("config.json"))
			{
				using ( StreamReader sr = new StreamReader( fs, new UTF8Encoding(false)) )
				{
					string jsonRaw = await sr.ReadToEndAsync();
					Config = JsonConvert.DeserializeObject<Config>(jsonRaw);
				}
			}
			Client = new DiscordClient(
				new DiscordConfiguration()
				{
					Token = Config.Token,
					TokenType = TokenType.Bot,
					MinimumLogLevel = LogLevel.Debug,
					Intents = DiscordIntents.AllUnprivileged
				}
			);
			SlashCommands = Client.UseSlashCommands(
				new SlashCommandsConfiguration() { }
			);
			Interactivity = Client.UseInteractivity(
				new InteractivityConfiguration() { Timeout = TimeSpan.FromSeconds(60) }
			);
			SlashCommands.RegisterCommands<Commands.Commands>();
			Client.ComponentInteractionCreated += ButtonPressResponse;
			await Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private Task ButtonPressResponse(DiscordClient client, ComponentInteractionCreateEventArgs eventArgs)
		{
			eventArgs.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource);
			return Task.CompletedTask;
		}

		public static bool AddGameInstance(DiscordChannel channel, Game game)
		{
			if ( gameInstances.ContainsKey(channel.Id) ) { return false; }
			gameInstances.Add(channel.Id, game);
			return true;
		}

		public static bool RemoveGameInstance(DiscordChannel channel)
		{
			return gameInstances.Remove(channel.Id);
		}

		public static Game? GetGameInstance(DiscordChannel channel)
		{
			foreach(KeyValuePair<ulong, Game> instance in gameInstances)
			{
				if (channel.Id == instance.Key) { return instance.Value; }
			}
			return null;
		}
	}
}
