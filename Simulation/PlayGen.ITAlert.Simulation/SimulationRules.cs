using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Common.Serialization;

namespace PlayGen.ITAlert.Simulation
{
	public class SimulationRules
	{
		[SyncState(StateLevel.Setup)]
		public bool VirusesDieWithSubsystem { get; set; }

		[SyncState(StateLevel.Setup)]
		public bool VirusesAlwaysVisible { get; set; }

		[SyncState(StateLevel.Setup)]
		public bool VirusesSpread { get; set; } = true;


		[SyncState(StateLevel.Setup)]
		public bool RepairItemsConsumable { get; set; }
	}
}
