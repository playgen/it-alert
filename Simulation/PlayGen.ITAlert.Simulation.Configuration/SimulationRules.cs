using Engine.Core.Serialization;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class SimulationRules
	{
		[SyncState(StateLevel.Setup)]
		public bool VirusesDieWithSystem { get; set; }

		[SyncState(StateLevel.Setup)]
		public bool VirusesAlwaysVisible { get; set; }

		[SyncState(StateLevel.Setup)]
		public bool VirusesSpread { get; set; } = true;


		[SyncState(StateLevel.Setup)]
		public bool RepairItemsConsumable { get; set; }
	}
}
