using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Configuration;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial;
using PlayGen.ITAlert.Simulation.Scenario;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class ScenarioLoader
	{
		private readonly Dictionary<string, ScenarioFactory> _scenarios;

		public ScenarioLoader()
		{
			// TODO: populate this from configuration
			_scenarios = new ScenarioFactory[]
			{
				new Tutorial1_Movement(),
				new Tutorial2_Analysis(),
				new Tutorial3_Bandwidth(),
				new Tutorial4_Mutation(),
				//new SPL1_35(),
				//new SPL1_50(),
				//new SPL1_65(),
				//new SPL1_80(),
				new SPL1(),
				new SPL2(),
				new SPL3(),
				//new Dev1(), 
			}
			.ToDictionary(k => k.ScenarioInfo.Key, v => v);
		}

		public bool TryGetScenario(string name, out SimulationScenario scenario)
		{
			if (_scenarios.TryGetValue(name, out var scenarioFactory))
			{
				scenario = scenarioFactory.GenerateScenario();
				return true;
			}
			scenario = null;
			return false;
		}

		public ScenarioInfo[] GetScenarioInfo()
		{
			return _scenarios.Values.Select(sf => sf.ScenarioInfo).ToArray();
		}

	}
}
