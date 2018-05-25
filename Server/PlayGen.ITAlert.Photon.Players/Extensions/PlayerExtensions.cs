using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Players.Extensions
{
	public static class PlayerExtensions
	{
		private static readonly Random Random = new Random();

		public static PlayerColour GetUnusedGlyph(this IEnumerable<ITAlertPlayer> players)
		{
			var glyph = PlayerColour.Glyphs.Except(players.Select(p => p.Glyph)).First();
			var glyphIndex = PlayerColour.Glyphs.ToList().IndexOf(glyph);
			var playerColour = new PlayerColour
			{
				Glyph = glyph,
				Colour = ConvertIntToHexColor(glyphIndex)
			};

			return playerColour;
		}

		public static string ConvertIntToHexColor(int i)
		{
			switch (i)
			{
				case 0:
					return "#E5297B";

				case 1:
					return "#EFF140";

				case 2:
					return "#46A7EB";

				case 3:
					return "#ED962F";

				case 4:
					return "#89C845";

				case 5:
					return "#E32730";

				default:
					return "#FFFFFF";
			}
		}
	}
}
