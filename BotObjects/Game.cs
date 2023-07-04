﻿
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot;
using Bot.BotHelpers;
using Bot.Commands;
using Alterblade;


namespace Bot.BotObjects
{
	internal class Game
	{
		public PVPBattleType BattleType { private set; get; }
		public DiscordChannel Channel { private set; get; }
		public List<DiscordPlayer> Players { private set; get; }
		public bool IsAccepted { private set; get; }

		public Game(PVPBattleType battleType, DiscordUser userA, DiscordUser userB, DiscordChannel channel)
		{
			BattleType = battleType;
			Channel = channel;
			Players = new List<DiscordPlayer>
			{
				new DiscordPlayer(userA),
				new DiscordPlayer(userB)
			};
			Bot.AddGameInstance(Channel, this);
		}

		public async Task StartSelection(InteractionContext ctx)
		{
			DiscordWebhookBuilder webhook = new DiscordWebhookBuilder()
				.WithContent($"Choose your heroes, {Players.First().User.Mention} and {Players.ElementAt(1).User.Mention}!")
				.AddEmbed(GameEmbedMaker.HeroListEmbed);
			await ctx.EditResponseAsync(webhook);
		}
		public async Task StartGameLoop(InteractionContext ctx)
		{
			DiscordButtonComponent btn = new DiscordButtonComponent(ButtonStyle.Primary, "he", "Yow");
			DiscordFollowupMessageBuilder followupBuilder = new DiscordFollowupMessageBuilder()
				.WithContent($"The battle commences! {Players.First().User.Mention} vs {Players.ElementAt(1).User.Mention}!")
				.AddComponents(btn);
			await ctx.FollowUpAsync(followupBuilder);
		}
		public async Task End(InteractionContext ctx, string content)
		{
			DiscordWebhookBuilder webhook = new DiscordWebhookBuilder();
			webhook.AddEmbed(new DiscordEmbedBuilder().WithTitle("Game Status").WithDescription(content));
			Bot.RemoveGameInstance(Channel);
			await ctx.EditResponseAsync(webhook);
		}

		public bool CheckIfPlayersAreReady()
		{
			bool ready = true;
			for (int i = 0; i < Players.Count; i++)
			{
				if (Players[i].Team.Count < 2) { ready = false; break; }
			}
			return ready;
		}
		public DiscordPlayer? GetPlayerFromUser(DiscordUser user)
		{
			for (int i = 0; i < Players.Count; i++)
			{
				DiscordPlayer player = Players.ElementAt(i);
				if ( player.User.Id == user.Id ) { return player; }
			}
			return null;
		}

	}
}
