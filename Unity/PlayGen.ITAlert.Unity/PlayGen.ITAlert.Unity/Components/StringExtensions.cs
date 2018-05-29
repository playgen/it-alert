using System.Linq;
using System.Text;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Components
{
	public static class StringExtensions
	{
		public static string Cutoff(this string stringToCut, char[] _cuttoffAfter, int maxLength)
		{
			var builder = new StringBuilder();
			for (var i = 0; i < Mathf.Min(stringToCut.Length, maxLength); i++)
			{
				if (!_cuttoffAfter.Contains(stringToCut[i]) && !_cuttoffAfter.Contains(stringToCut[i]))
				{
					builder.Append(stringToCut[i]);
				}
				else
				{
					break;
				}
			}

			if (stringToCut.Length > maxLength && builder.ToString().Length == maxLength)
			{
				builder.Append("...");
			}

			return builder.ToString();
		}
	}
}
