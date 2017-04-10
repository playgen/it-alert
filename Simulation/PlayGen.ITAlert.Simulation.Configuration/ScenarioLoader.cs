using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Configuration;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial;
using PlayGen.ITAlert.Simulation.Scenario;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class ScenarioLoader
	{
		private readonly Dictionary<string, SimulationScenario> _scenarios;

		public ScenarioLoader()
		{
			// TODO: populate this from configuration
			_scenarios = new[]
			{
				Tutorial1_Introduction.Scenario,
				Tutorial2_Analysis.Scenario,
				Tutorial3_Bandwidth.Scenario,
				Tutorial4_Mutation.Scenario,
			}
			//.Concat(GraphDemos.Scenarios)
			.ToDictionary(k => k.Key, v => v);
		}

		public bool TryGetScenario(string name, out SimulationScenario scenario)
		{
			return _scenarios.TryGetValue(name, out scenario);
		}

		public ScenarioInfo[] GetScenarioInfo()
		{
			return _scenarios.Values.Select(s => s.ToScenarioInfo()).ToArray();
		}

	}
}
