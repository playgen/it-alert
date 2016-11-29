using System;

namespace Engine.Messaging
{
	[Flags]
	public enum MessageScope
	{
		Internal = 1,
		External = 2,
	}
}
