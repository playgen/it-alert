using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario
{
	public static class ScenarioExtensions
	{
		public static ScenarioInfo ToScenarioInfo(this SimulationScenario scenario)
		{
			return new ScenarioInfo()
			{
				Key = scenario.Key,
				Name = scenario.Name,
				Description = scenario.Description,
				MaxPlayerCount = scenario.MaxPlayers,
				MinPlayerCount = scenario.MinPlayers,
				LocalizationDictionary = scenario.LocalizationDictionary,
			};
		}
	}
}
