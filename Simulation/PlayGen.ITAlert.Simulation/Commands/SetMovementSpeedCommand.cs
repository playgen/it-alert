using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Commands;

using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Systems.Movement;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class SetMovementSpeedCommand : ICommand
	{
		public decimal Delta { get; set; }

		public MovementOffsetCategory Category { get; set; }
	}

	public class SetMovementSpeedCommandHandler : CommandHandler<SetMovementSpeedCommand>
	{
		private readonly MovementSpeedSystem _movementSpeedSystem;

		public SetMovementSpeedCommandHandler(MovementSpeedSystem movementSpeedSystem)
		{
			_movementSpeedSystem = movementSpeedSystem;
		}

		protected override bool TryHandleCommand(SetMovementSpeedCommand command, int currentTick, bool handlerEnabled)
		{
			foreach (MovementOffsetCategory cat in Enum.GetValues(typeof(MovementOffsetCategory)))
			{
				if ((command.Category & cat) != 0)
				{
					if (_movementSpeedSystem.SpeedOffsets.ContainsKey(cat))
					{
						_movementSpeedSystem.SpeedOffsets[cat] += command.Delta;
					}
					else
					{
						_movementSpeedSystem.SpeedOffsets.Add(cat, command.Delta);
					}
				}
			}
			return true;
		}
	}
}
