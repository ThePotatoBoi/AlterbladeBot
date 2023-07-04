using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Bot.BotHelpers;
using Bot.BotObjects;
using Alterblade;
using Alterblade.GameObjects;
using Bot.Commands.ChoiceProviders;

namespace Bot.Commands
{
	internal class Commands : ApplicationCommandModule
	{

		[SlashCommand("pvpbattle", "Battle a fellow KOMRAD in an Alterblade match!")]
		public async Task PVPBattle(
			InteractionContext ctx,
			[Option("user", "Select a fellow KOMRAD to battle.")] DiscordUser targetUser,
			[Option("battletype", "What kind of battle do you want?")] PVPBattleType battleType
		) {
			DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

			if (ctx.User.Id == targetUser.Id)
			{
				embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "Welp. Who you tryin' to battle with? Keep it to yourself, man.");
				responseBuilder.IsEphemeral = true;
#if !DEBUG
			}
			else if ( targetUser.IsBot )
			{ 
				embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "Seriously? You can't battle bots!");
				responseBuilder.IsEphemeral = true;
#endif
			}
			else if (Bot.GetGameInstance(ctx.Channel) != null)
			{
				embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "A game instance is present in this channel. Try other channels or wait for them to finish.");
				responseBuilder.IsEphemeral = true;
			}
			else
			{
				embedBuilder
					.WithColor(DiscordColor.Yellow)
					.WithTitle("Battle Invitation")
					.WithDescription($"{ctx.Member.Mention} invites you to an Alterblade battle! Do you consent your honor in a match?")
					.AddField("Battle Type", battleType.GetName(), true)
					.WithFooter(
						"This invitation will expire within a minute.",
						"https://cdn-0.emojis.wiki/emoji-pics/twitter/question-mark-twitter.png"
					);
				responseBuilder
					.WithContent($"Hey {targetUser.Mention}!")
					.AddComponents(
						new DiscordButtonComponent(ButtonStyle.Primary, "pvpBattle_consent", "Accept")
					);
			}

			await ctx.CreateResponseAsync(
				InteractionResponseType.ChannelMessageWithSource,
				responseBuilder.AddEmbed(embedBuilder.Build())
			);

			if ( responseBuilder.Components.Count > 0 )
			{
				DiscordMessage msg = await ctx.GetOriginalResponseAsync();
				DiscordFollowupMessageBuilder followup = new DiscordFollowupMessageBuilder();
				while (true)
				{
					InteractivityResult<ComponentInteractionCreateEventArgs> buttonPressResult = await msg.WaitForButtonAsync(TimeSpan.FromSeconds(60));
					if (buttonPressResult.TimedOut)
					{
						embedBuilder = GameEmbedMaker.ErrorEmbed("Expired", "Aight... maybe next time, then?");
						followup.AddEmbed(embedBuilder);
						followup.IsEphemeral = true;
						Bot.RemoveGameInstance(ctx.Channel);
						await ctx.FollowUpAsync(followup);
						await msg.DeleteAsync();
						break;
					}
					else if (buttonPressResult.Result.User.Id != targetUser.Id)
					{
						embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "This invitation is not for you.");
						followup.AddEmbed(embedBuilder);
						followup.IsEphemeral = true;
						await ctx.FollowUpAsync(followup);
					}
					else
					{
						Game game = new Game(battleType, ctx.User, targetUser, ctx.Channel);
						embedBuilder = new DiscordEmbedBuilder()
							.WithColor(DiscordColor.Green)
							.WithTitle("Invitation Accepted!")
							.WithDescription($"{targetUser.Mention} accepted the invitation. Prepare for a match!");
						await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedBuilder));
						await Task.Delay(TimeSpan.FromSeconds(4));
						await game.StartSelection(ctx);
						break;
					}
				}
			}

		}

		[SlashCommand("selectheroes", "Select your heroes for an upcoming match!")]
		public async Task SelectHeroes(
			InteractionContext ctx,
			[ChoiceProvider(typeof(HeroChoiceProvider))] [Option("firsthero", "Choose your first hero.")] string heroAName,
			[ChoiceProvider(typeof(HeroChoiceProvider))] [Option("secondhero", "Choose your second hero.")] string heroBName
		) {
			DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

			Game? game = Bot.GetGameInstance(ctx.Channel);
			DiscordPlayer? player = game?.GetPlayerFromUser(ctx.User);

			if (game == null)
			{
				embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "No game instance is present. Maybe start a battle with `\\pvpBattle` first?");
				responseBuilder.IsEphemeral = true;
			}
			else if (player == null)
			{
				embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "You are not a player in the game instance.");
				responseBuilder.IsEphemeral = true;
			}
			else
			{
				Hero? heroA = GameConstants.GetHeroFromName(heroAName);
				Hero? heroB = GameConstants.GetHeroFromName(heroBName);

				if ( heroA == null || heroB == null )
				{
					embedBuilder = GameEmbedMaker.ErrorEmbed("Error", "There was an error registering your heroes.");
					responseBuilder.IsEphemeral = true;
					Bot.RemoveGameInstance(ctx.Channel);
				}
				else
				{
					player.Team.Add(heroA);
					player.Team.Add(heroB);
					embedBuilder
						.WithTitle("Ready!")
						.WithColor(DiscordColor.DarkButNotBlack)
						.WithDescription($"Your heroes {heroA.Name} and {heroB.Name} are set. The game will start once both players are ready.");
					responseBuilder.IsEphemeral = true;
				}
			}

			await ctx.CreateResponseAsync(
				InteractionResponseType.ChannelMessageWithSource,
				responseBuilder.AddEmbed(embedBuilder.Build())
			);

			if ( game != null && game.CheckIfPlayersAreReady() )
			{
				await Task.Delay(TimeSpan.FromSeconds(2));
				await game.StartGameLoop(ctx);
			}
		}
	}
}
