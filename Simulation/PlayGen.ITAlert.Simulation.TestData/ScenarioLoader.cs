using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class ScenarioLoader
	{
		private readonly Scenario[] _scenarios = new Scenario[]
		{
			GameScenarios.Introduction,
		};

		public IEnumerable<Scenario> GetScenarios()
		{
			return _scenarios;
		}

	}
}
