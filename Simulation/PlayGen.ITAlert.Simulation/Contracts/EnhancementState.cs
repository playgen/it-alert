using PlayGen.ITAlert.Configuration;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class EnhancementState : ITAlertEntityState
	{
		public EnhancementType EnhancementType { get; }

		public bool Active { get; set; }

		public int ActiveDuration { get; set; }

		public int ActiveTicksRemaining { get; set; }

		public EnhancementState(int id, EnhancementType enhancementType) 
			: base(id, EntityType.Enhancement)
		{
			EnhancementType = enhancementType;
		}
	}
}