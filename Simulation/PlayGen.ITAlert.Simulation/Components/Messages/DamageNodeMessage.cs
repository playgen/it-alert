using Engine.Messaging;

namespace PlayGen.ITAlert.Simulation.Components.Messages
{
	public class DamageNodeMessage : Message
	{
		public DamageNodeMessage(MessageScope scope) : base(scope)
		{
		}
	}
}
