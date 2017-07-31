using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Engine.Commands;

using PlayGen.ITAlert.Simulation.Systems.Movement;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class SetPlayerMovementSpeedCommand : ICommand
	{
		public decimal Delta { get; set; }
	}

	public class SetPlayerMovementSpeedCommandHandler : CommandHandler<SetPlayerMovementSpeedCommand>
	{
		private readonly MovementSpeedSystem _movementSpeedSystem;

		public SetPlayerMovementSpeedCommandHandler(MovementSpeedSystem movementSpeedSystem)
		{
			_movementSpeedSystem = movementSpeedSystem;
		}

		protected override bool TryHandleCommand(SetPlayerMovementSpeedCommand command, int currentTick, bool handlerEnabled)
		{
			_movementSpeedSystem.PlayerSpeedOffset += command.Delta;
			return true;
		}
	}
}
