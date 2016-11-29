using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Messaging
{
	public class TickMessage : Message
	{
		public int CurrentTick { get; }

		public TickMessage(int currentTick)
			: base(MessageScope.Internal)
		{
			CurrentTick = currentTick;
		}
	}
}
