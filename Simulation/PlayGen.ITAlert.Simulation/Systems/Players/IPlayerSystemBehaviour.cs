using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Systems.Players
{
	public interface IPlayerSystemBehaviour : ISystemExtension
	{
		void OnPlayerJoined(int playerEntityId);

		void OnPlayerDisconnected(int playerEntityId);

	}
}
