using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Messaging;

namespace Engine.Components.Messaging
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
