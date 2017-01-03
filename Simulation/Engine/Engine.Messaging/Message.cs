using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Messaging
{
	public abstract class Message : IMessage
	{
		public IMessageHub Origin { get; private set; }


		public MessageScope Scope { get; set; }
		
		protected Message(MessageScope scope)
		{
			Scope = scope;
		}
		public void SetOrigin(IMessageHub origin)
		{
			if (Origin == null)
			{
				Origin = origin;
			}
			//throw new Exception("message origin alreadty set");
		}
	}
}
