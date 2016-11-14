using Engine.Core.Messaging;

namespace PlayGen.ITAlert.Simulation.Systems.Messages
{
	public class DamageNodeMessage : Message
	{
		public DamageNodeMessage(MessageScope scope) : base(scope)
		{
		}
	}
}
