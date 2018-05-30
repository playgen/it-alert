using System;
using Engine.Commands;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands
{
	public class SetCommandEnabledCommand : ICommand
	{
		public Type Command { get; set; }

		public bool Enabled { get; set; }
	}

	public class SetCommandEnabledCommandHandler : CommandHandler<SetCommandEnabledCommand>
	{
		public SetCommandEnabledCommandHandler()
		{
		}

		protected override bool TryHandleCommand(SetCommandEnabledCommand command, int currentTick, bool handlerEnabled)
		{
			if (handlerEnabled && typeof(ICommand).IsAssignableFrom(command.Command))
			{
				
			}
			return false;
		}
	}
}
