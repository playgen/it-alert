namespace Engine.Messaging
{
	public interface IMessage
	{
		IMessageHub Origin { get; }

		void SetOrigin(IMessageHub origin);

		MessageScope Scope { get; set; }
	}
}
