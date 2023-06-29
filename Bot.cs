using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DSharpPlus.EventArgs;

namespace PrototypeA
{
	internal class Bot
	{
		public DiscordClient Client { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public SlashCommandsExtension SlashCommands { get; private set; }
		public Config Config { get; private set; }

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
	}
}
