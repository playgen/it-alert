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
		public Dictionary<MovementOffsetCategory, decimal> SpeedOffsets { get; set; } = new Dictionary<MovementOffsetCategory, decimal>();

		public decimal GetMovementSpeed(MovementSpeed movementSpeed)
		{
			var offsetSpeed = movementSpeed.Value;
			foreach (MovementOffsetCategory cat in Enum.GetValues(typeof(MovementOffsetCategory)))
			{
				if ((movementSpeed.Category & cat) != 0 && SpeedOffsets.TryGetValue(cat, out var delta))
				{
					offsetSpeed += delta;
				}
			}
			return offsetSpeed < 0.1m ? 0.1m : offsetSpeed;
		}

		public void Dispose()
		{
		}
	}
}
