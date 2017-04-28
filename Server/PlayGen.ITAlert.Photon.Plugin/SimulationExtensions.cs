using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Systems.Initialization;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Photon.Plugin
{
	public static class SimulationExtensions
	{
		public static void PlayerDisconnected(this Simulation.Simulation simulation, int photonPlayerId)
		{
			simulation.EnqueueCommand(new PlayerDisconnectedCommand()
			{
				PlayerExternalId = photonPlayerId,
			});

		}

	}
}
