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
			var unusedGlyphs = PlayerColour.Glyphs.Except(players.Select(p => p.Glyph)).ToArray();
			var glyph = unusedGlyphs[Random.Next(0, unusedGlyphs.Length)];
			var glyphIndex = PlayerColour.Glyphs.ToList().IndexOf(glyph);
			var playerColour = new PlayerColour
									{
				Glyph = glyph,
				Colour = ConvertHueToRgb(glyphIndex)
			};

			return playerColour;
		}

		public static string ConvertHueToRgb(double h)
		{
			double r, g, b;

			var i = (int)(h);
			var f = h - i;

			double p = 0;
			var q = (1.0 - (1 * f));
			var t = (1.0 - (1 * (1.0f - f)));

			switch (i)
			{
				case 0:
					r = 1;
					g = t;
					b = p;
					break;

				case 1:
					r = q;
					g = 1;
					b = p;
					break;

				case 2:
					r = p;
					g = 1;
					b = t;
					break;

				case 3:
					r = p;
					g = q;
					b = 1;
					break;

				case 4:
					r = t;
					g = p;
					b = 1;
					break;

				default:
					r = 1;
					g = p;
					b = q;
					break;
			}
			var rByte = (int) r * 255;
			var gByte = (int) g * 255;
			var bByte = (int) b * 255;

			return $"#{rByte:X02}{gByte:X02}{bByte:X02}";

		}
	}
}
