using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Commands;
using Engine.Sequencing;

namespace PlayGen.ITAlert.Simulation.Extensions
{
	public static class SystemExtensions
	{
		public static bool TryHandleCommand(this IECS ecs, ICommand command)
		{
			CommandSystem commandSystem;
			if (ecs.TryGetSystem(out commandSystem))
			{
				return commandSystem.TryHandleCommand(command);
			}
			return false;
		}
	}
}
