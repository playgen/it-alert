using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class SetCommandEnabled<TCommand> : SimulationAction
		where TCommand : ICommand
	{
		public SetCommandEnabled(bool enabled)
			: base($"Set command enabled ({nameof(TCommand)}): {enabled}")
		{
			Action = (ecs, config) =>
			{
				ICommandSystem commandSystem;
				ICommandHandler commandHandler;
				if (ecs.TryGetSystem(out commandSystem)
					&& commandSystem.TryGetHandler<TCommand>(out commandHandler))
				{
					commandHandler.SetEnabled(enabled);
				}
			};
		}
	}
}
