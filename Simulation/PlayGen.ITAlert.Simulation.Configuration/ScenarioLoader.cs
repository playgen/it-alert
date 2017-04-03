using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Configuration;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial;

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
				BigGraphTest.Scenario,
			}
			.Concat(GraphDemos.Scenarios)
			.ToDictionary(k => k.Name, v => v);
		}

		public bool TryGetScenario(string name, out SimulationScenario scenario)
		{
			return _scenarios.TryGetValue(name, out scenario);
		}

		public ScenarioInfo[] GetScenarioInfo()
		{
			return _scenarios.Values.Select(s => new ScenarioInfo()
			{
				Name = s.Name,
				Description = s.Description,
				MaxPlayerCount = s.MaxPlayers,
				MinPlayerCount = s.MinPlayers,
			}).ToArray();
		}

	}
}
