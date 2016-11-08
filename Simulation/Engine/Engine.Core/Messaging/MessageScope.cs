using System;

namespace Engine.Core.Messaging
{
	[Flags]
	public enum MessageScope
	{
		Internal = 1,
		External = 2,
	}
}
