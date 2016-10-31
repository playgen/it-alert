using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Messaging;

namespace PlayGen.Engine.Components.Messaging
{
	public interface IMessageComponent : IComponent
	{
	}

	public interface IMessageComponent<TMessage> : IMessageComponent
		where TMessage : IMessage
	{
		
	}
}
