using System.Collections.Generic;

namespace PlayGen.Photon.Players.Extensions
{
	public static class PlayerExtensions
	{
		public static bool AnyHasColor(this IEnumerable<Player> players, string color)
		{
			var hasColor = false;

			foreach (var player in players)
			{
				if (player.Color == color)
				{
					hasColor = true;
					break;
				}
			}

			return hasColor;
		}

		public static string GetUnusedColor(this IEnumerable<Player> players)
		{
			string unusedColor = null;

			foreach (var color in PlayerColors.Colors)
			{
				if (!players.AnyHasColor(color))
				{
					unusedColor = color;
					break;
				}
			}

			return unusedColor;
		}
	}
}
