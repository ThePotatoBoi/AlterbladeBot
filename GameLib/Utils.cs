using AlterbladeBot.GameLib.GameObjects;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace AlterbladeBot.GameLib
{
	internal static class Utils
	{

		public static readonly Random Random = new Random();

		/// <summary>
		/// Console Color Helper provides coloring to individual Console commands!
		/// Credits to @RickStrahl from https://gist.github.com/RickStrahl/52c9ee43bd2723bcdf7bf4d24b029768
		/// </summary>
		#region ColorConsole

		readonly static Lazy<Regex> colorBlockRegEx = new Lazy<Regex>(
			delegate () { return new Regex("\\[(?<color>.*?)\\](?<text>[^[]*)\\[/\\k<color>\\]", RegexOptions.IgnoreCase); },
			isThreadSafe: true
		);

		/*public static void WriteLine(string text, ConsoleColor? color = null)
		{
			if (color.HasValue)
			{
				var oldColor = Console.ForegroundColor;
				if (color == oldColor)
					Console.WriteLine(text);
				else
				{
					Console.ForegroundColor = color.Value;
					Console.WriteLine(text);
					Console.ForegroundColor = oldColor;
				}
			}
			else Console.WriteLine(text);
		}

		public static void Write(string text, ConsoleColor? color = null, bool isColorBackground = false)
		{
			if (color.HasValue)
			{
				var oldColor = Console.ForegroundColor;
				var oldBgColor = Console.BackgroundColor;
				if (color == oldColor)
					Console.Write(text);
				else
				{
					if (isColorBackground) { Console.BackgroundColor = color.Value; } else { Console.ForegroundColor = color.Value; }
					Console.Write(text);
					if (isColorBackground) { Console.BackgroundColor = oldBgColor; } else { Console.ForegroundColor = oldColor; }
				}
			}
			else Console.Write(text);
		}

		public static void Write(string text, string color, bool isColorBackground = false)
		{
			if (string.IsNullOrEmpty(color))
			{
				Write(text);
				return;
			}

			if (!Enum.TryParse(color, true, out ConsoleColor col))
				Write(text);
			else
				Write(text, col, isColorBackground);
		}

		public static void Error(string str, bool terminate = false)
		{
			WriteLine(str, ConsoleColor.Red);
			if (terminate) Environment.Exit(0);
		}

		public static void Warn(string str)
		{
			WriteLine(str, ConsoleColor.Yellow);
		}

		public static void WriteLineColor(string text, ConsoleColor? baseTextColor = null)
		{
			WriteEmbeddedColor(text, baseTextColor);
			Console.WriteLine();
		}

		public static void WriteEmbeddedColor(string text, ConsoleColor? baseTextColor = null)
		{
			if (baseTextColor == null) { baseTextColor = Console.ForegroundColor; }

			if (string.IsNullOrEmpty(text))
			{
				Write(string.Empty);
				return;
			}

			int at = text.IndexOf("[");
			int at2 = text.IndexOf("]");
			if (at == -1 || at2 <= at)
			{
				Write(text, baseTextColor);
				return;
			}

			while (true)
			{
				var match = colorBlockRegEx.Value.Match(text);
				if (match.Length < 1)
				{
					Write(text, baseTextColor);
					break;
				}
				bool isBg = false;
				Write(text.Substring(0, match.Index), baseTextColor);
				string highlightText = match.Groups["text"].Value;
				if (highlightText.Length > 0 && highlightText[0] == '~')
				{
					isBg = true;
					highlightText = highlightText.Substring(1);
				}
				string colorVal = match.Groups["color"].Value;
				Write(highlightText, colorVal, isBg);
				text = text.Substring(match.Index + match.Value.Length);
			}

		}*/

		#endregion

		public static void ClearScreen()
		{
#if DEBUG
				WriteLineColor("\n[red]~ <Clear Screen> [/red]\n");
#else
				Console.Clear();
				Console.WriteLine();
#endif
		}

		public static void Delay(int ms)
		{
			if (ms < 1) { return; }
#if DEBUG
			Error(new StringBuilder().AppendFormat("Wait {0}ms", ms).ToString());
#else
			Thread.Sleep(TimeSpan.FromMilliseconds(ms));
			while (Console.KeyAvailable)
			{
				Console.ReadKey(true);
			}
#endif

		}

		public static void DelayedWrite(string str, int ms)
		{
			Delay(ms);
			WriteLineColor(str);
		}

		public static void DelayedWrite(StringBuilder str, int ms)
		{
			Delay(ms);
			WriteLineColor(str.ToString());
		}

		public static void DelayedError(string str, int ms, bool terminate = false)
		{
			Delay(ms);
			Error(str, terminate);
		}

		public static int GetInteger(int min, int max, string query = "")
		{
			while (true)
			{
				if (query.Length > 0)
					WriteEmbeddedColor(new StringBuilder().Append("█ ").Append(query).ToString());
				if (int.TryParse(Console.ReadLine(), out int input))
				{
					if (input >= min && input <= max) return input;
				}
				Error("Input is invalid. Please try again.");
			}
		}

		public static bool RollBoolean(float chance)
		{
			return Random.NextDouble() < chance;
		}

		public static int CalculateDamage(int baseDamage, bool ignoreDefense, int attackerAttack, int targetDefense, int targetBaseDefense)
		{
			int defense = ignoreDefense ? Math.Clamp(targetDefense, 0, targetBaseDefense) : targetDefense;
			float staple = (3F * baseDamage * attackerAttack / (defense * 5F)) + 12F;
			return Convert.ToInt32(staple);
		}

		public static int CalculateDamage(int baseDamage, bool isCrit, Hero attacker, Hero target)
		{
			return CalculateDamage(
				baseDamage, false,
				attacker.CurrentStats[Stats.ATTACK],
				target.CurrentStats[Stats.DEFENSE],
				target.BaseStats[Stats.DEFENSE]
			);
		}

		public static bool RollAccuracy(float accuracy, float targetEvasiveness, bool showText)
		{
			if (accuracy <= 0) return true;
			bool willHit = RollBoolean(accuracy - targetEvasiveness);
			if (!willHit && showText)
			{
				DelayedError("The attack missed!", 1000);
			}
			return willHit;
		}
	}
}