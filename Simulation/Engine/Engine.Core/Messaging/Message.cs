using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Components;

namespace Engine.Core.Messaging
{
	public abstract class Message : IMessage
	{
		public IMessageHub Origin { get; set; }

		public MessageScope Scope { get; }
		
		protected Message(MessageScope scope)
		{
			Scope = scope;
		}
	}
}
