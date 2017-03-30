using System;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class InitializedMessage : SimulationMessage
	{
		public string SimulationState { get; set; }

		public string PlayerConfiguration { get; set; }

		public Guid InstanceId { get; set; }

		public string ScenarioName { get; set; }
	}
}
