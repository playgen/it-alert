using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Scenario;
using PlayGen.ITAlert.Simulation.Scenario.Localization;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios
{
	public abstract class ScenarioFactory
	{
		public ScenarioInfo ScenarioInfo { get; private set; }

		protected ScenarioFactory(ScenarioInfo scenarioInfo)
		{
			ScenarioInfo = scenarioInfo;
		}

		protected ScenarioFactory(string key,
			string nameToken,
			string descriptionToken,
			int minPlayers,
			int maxPlayers,
            int? timeLimitSeconds)
			: this(new ScenarioInfo
			{
				Description = descriptionToken,
				Key = key,
				LocalizationDictionary = LocalizationHelper.GetLocalizationFromEmbeddedResource(key),
				MaxPlayerCount = maxPlayers,
				MinPlayerCount = minPlayers,
				Name = nameToken,
                TimeLimitSeconds = timeLimitSeconds
            })
		{
			
		}


		public abstract SimulationScenario GenerateScenario();
	}
}
