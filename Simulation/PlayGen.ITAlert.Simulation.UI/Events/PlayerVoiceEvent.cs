using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.UI.Events
{
	public class PlayerVoiceEvent : PlayerEvent
	{
		public enum Signal
		{
			Error = 0,
			Activated,
			Deactivated,
		}

		public Signal Mode { get; set; }
	}
}
