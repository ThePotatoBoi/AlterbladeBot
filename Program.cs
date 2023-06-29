using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Threading.Tasks;
using PrototypeA.Commands;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace PrototypeA
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			Bot bot = new Bot();
			await bot.RunAsync();
		}
	}
}