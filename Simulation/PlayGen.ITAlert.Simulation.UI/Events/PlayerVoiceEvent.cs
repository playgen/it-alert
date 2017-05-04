using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.UI.Events
{
	public class PlayerVoiceEvent : Event, IPlayerEvent
	{
		public enum Signal
		{
			Error = 0,
			Activated,
			Deactivated,
		}

		public Signal Mode { get; set; }
		public int PlayerEntityId { get; set; }
	}
}
