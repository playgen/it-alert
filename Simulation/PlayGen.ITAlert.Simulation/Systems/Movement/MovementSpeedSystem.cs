using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Systems;

using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Events;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Movement
{
	public class MovementSpeedSystem : ITickableSystem
	{
		private readonly EventSystem _eventSystem;

		/// <summary>
		/// 
		/// </summary>
		public Dictionary<MovementOffsetCategory, decimal> SpeedOffsets { get; set; } = new Dictionary<MovementOffsetCategory, decimal>();

		public MovementSpeedSystem(EventSystem eventSystem)
		{
			_eventSystem = eventSystem;
		}

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

		public void LeaveSystem(int id)
		{
			var @event = new PlayerLeaveNodeEvent()
			{
				PlayerEntityId = id
			};
			_eventSystem.Publish(@event);

		}

		public void Dispose()
		{
		}

		public void Tick(int currentTick)
		{
			
		}
	}
	public class PlayerLeaveNodeEvent : Event, IPlayerEvent
	{
		public int PlayerEntityId { get; set; }
	}
}
