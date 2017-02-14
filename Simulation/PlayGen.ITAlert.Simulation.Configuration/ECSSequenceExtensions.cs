using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Commands;
using Engine.Sequencing;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	// ReSharper disable once InconsistentNaming
	public static class ECSSequenceExtensions
	{
		public static void HandleCommand(this IECS ecs, ICommand command)
		{
			ecs.EnqueueCommand(command);
		}
	}
}
