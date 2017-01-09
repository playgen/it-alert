using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Serialization;
using PlayGen.ITAlert.Simulation.TestData;

namespace PlayGen.ITAlert.Simulation.CLI
{
	public class Program
	{
		private static readonly string DebugPath = Path.Combine(Directory.GetCurrentDirectory(), "output");


		public static void Main(string[] args)
		{
			var simulation = ConfigHelper.GenerateSimulation(3, 3, 3, 3, 1);

			var state = simulation.GetState();

			VisitorPositionState visitorPositionState;
			var playerLocations = state.EntityStates.Where(s => s.Value.TryGet(out visitorPositionState) && visitorPositionState.Any());

			simulation.Tick();
		}
	}
}
