using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeA.Commands
{
	enum PVPBattleType
	{
		[ChoiceName("Hero vs Hero")]
		NORMAL,
		[ChoiceName("Team vs Team")]
		TEAM
	}

	internal class Commands : ApplicationCommandModule
	{

		[SlashCommand("pvpBattle", "Battle a fellow KOMRAD in an Alterblade match!")]
		public async Task CreatePVPBattle(
			InteractionContext ctx,
			[Option("user", "Select a fellow KOMRAD to battle.")] DiscordUser targetUser,
			[Option("battleType", "What kind of battle do you want?")] PVPBattleType battleType
		) {

			DiscordInteractionResponseBuilder responseBuilder = new DiscordInteractionResponseBuilder();
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();

			if ( ctx.User.Id == targetUser.Id ) {

				embedBuilder
					.WithColor(DiscordColor.Red)
					.WithTitle("Error")
					.WithDescription("Welp. Who you tryin' to battle with? Keep it to yourself, man.");
#if !DEBUG
			} else if ( targetUser.IsBot ) { 
							
				embedBuilder
					.WithColor(DiscordColor.Red)
					.WithTitle("Error")
					.WithDescription("Seriously? You can't battle bots!");
#endif
			} else {

				DiscordButtonComponent consentButton = new DiscordButtonComponent(
					ButtonStyle.Primary,
					"pvpBattle_consent",
					"Accept"
				);

				embedBuilder
					.WithColor(DiscordColor.Yellow)
					.WithTitle("Battle Invitation")
					.WithDescription($"{ctx.Member.Mention} invites you to an Alterblade battle! Do you consent your honor in a match?")
					.AddField("Battle Type", battleType.GetName(), true)
					.WithFooter(
						"This invitation will expire within a minute.",
						"https://cdn-0.emojis.wiki/emoji-pics/twitter/question-mark-twitter.png"
					);

				responseBuilder.WithContent($"Hey {targetUser.Mention}!")
					.WithContent($"Hey {targetUser.Mention}!")
					.AddComponents(consentButton);

			}

			await ctx.CreateResponseAsync(
				InteractionResponseType.ChannelMessageWithSource,
				responseBuilder.AddEmbed(embedBuilder.Build())
			);

			if ( responseBuilder.Components.Count > 0 )
			{
				DiscordMessage msg = await ctx.GetOriginalResponseAsync();
				InteractivityResult<ComponentInteractionCreateEventArgs> buttonPressResult = await msg.WaitForButtonAsync();

				if ( !buttonPressResult.TimedOut )
				{
					// PVPBattle gameInstance = new PVPBattle(battleType, ctx.User, targetUser);
					bool endGame = false;
					int loopCount = 0;

					while (!endGame)
					{

						DiscordWebhookBuilder webhook = new DiscordWebhookBuilder();

						webhook.AddEmbed( new DiscordEmbedBuilder().WithAuthor(loopCount.ToString()).WithDescription("HAHA") );
						webhook.AddComponents( new DiscordButtonComponent(ButtonStyle.Primary, "ha", loopCount.ToString()) );

						msg = await ctx.EditResponseAsync( webhook );
						buttonPressResult = await msg.WaitForButtonAsync();

						if ( !buttonPressResult.TimedOut ) { loopCount++; } else { endGame = true; }
						
					}
				}

			}

		}
	}
}
