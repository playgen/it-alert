using Engine.Core.Components;
using Engine.Core.Messaging;

namespace Engine.Components.Messaging
{
	public interface IMessageComponent : IComponent
	{
	}

	public interface IMessageComponent<TMessage> : IMessageComponent
		where TMessage : IMessage
	{
		
	}
}
