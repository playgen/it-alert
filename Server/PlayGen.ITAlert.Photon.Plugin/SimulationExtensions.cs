using PlayGen.ITAlert.Simulation.Commands;

namespace PlayGen.ITAlert.Photon.Plugin
{
	public static class SimulationExtensions
	{
		public static void PlayerDisconnected(this Simulation.Simulation simulation, int photonPlayerId)
		{
			simulation.EnqueueCommand(new PlayerDisconnectedCommand
										{
				PlayerExternalId = photonPlayerId
			});

		}

	}
}
