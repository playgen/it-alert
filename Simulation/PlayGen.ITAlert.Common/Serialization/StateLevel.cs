using System;

namespace PlayGen.ITAlert.Common.Serialization
{
	[Flags]
	public enum StateLevel
	{
		Undefined = 0,
		Setup = 1,
		Minimal = 2,
		Full = 3,
		Ui = 4,
	}

	public static class StateLevelExtensions
	{
		public static bool IncludesFlag(this StateLevel stateLevel, StateLevel flag)
		{
			return (stateLevel & flag) == flag;
		}
	}
}
