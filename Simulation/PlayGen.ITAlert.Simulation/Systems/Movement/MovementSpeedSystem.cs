using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Systems;

using PlayGen.ITAlert.Simulation.Components.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class MovementSpeedSystem : ISystem
	{
		/// <summary>
		/// 
		/// </summary>
		public decimal PlayerSpeedOffset { get; set; }

		public decimal GetMovementSpeed(MovementSpeed movementSpeed)
		{
			return movementSpeed.Value + PlayerSpeedOffset;
		}

		public void Dispose()
		{
		}
	}
}
