using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Sequencing;
using PlayGen.ITAlert.Simulation.Scenario.Localization;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class SimulationScenario : Scenario<Simulation, SimulationConfiguration>
	{
		public enum ScoringMode
		{
			None = 0,
			Team,
			Player,
			Full,
		}

		public IPlayerConfigFactory PlayerConfigFactory { get; set; }

		/// <summary>
		/// Dictionary keyed by country-code, then by text key
		/// </summary>
		public LocalizationDictionary LocalizationDictionary { get; set; }

		public int? TimeLimitSeconds { get; set; }

		public ScoringMode Scoring { get; set; }

		public SimulationScenario()
		{
			
		}

		public SimulationScenario(ScenarioInfo scenarioInfo)
		{
			Key = scenarioInfo.Key;
			Name = scenarioInfo.Name;
			Description = scenarioInfo.Description;
			MinPlayers = scenarioInfo.MinPlayerCount;
			MaxPlayers = scenarioInfo.MaxPlayerCount;
			LocalizationDictionary = scenarioInfo.LocalizationDictionary;
		}
	}
}
