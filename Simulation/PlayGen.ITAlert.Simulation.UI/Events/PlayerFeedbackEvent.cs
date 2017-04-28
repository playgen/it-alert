using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.UI.Events
{
	public class PlayerFeedbackEvent : Event
	{
		public int PlayerExternalId { get; set; }

		public int RankedPlayerExternalId { get; set; }

		public int[] PlayerRankings { get; set; }
	}
}
