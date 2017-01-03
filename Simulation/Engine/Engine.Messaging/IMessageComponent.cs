using Engine.Components;

namespace Engine.Messaging
{
	public interface IMessageComponent : IComponent
	{
	}

	public interface IMessageComponent<TMessage> : IMessageComponent
		where TMessage : IMessage
	{
		
	}
}
