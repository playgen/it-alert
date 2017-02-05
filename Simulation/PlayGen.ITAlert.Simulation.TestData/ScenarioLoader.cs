using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class ScenarioLoader
	{
		private readonly Dictionary<string, SimulationScenario> _scenarios;

		public ScenarioLoader()
		{
			// TODO: populate this from configuration
			_scenarios = new Dictionary<string, SimulationScenario>()
			{
				{GameScenarios.Introduction.Name, GameScenarios.Introduction}
			};
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
