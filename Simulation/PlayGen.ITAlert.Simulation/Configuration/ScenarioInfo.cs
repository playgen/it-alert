using PlayGen.ITAlert.Simulation.Scenario.Localization;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class ScenarioInfo
	{
		public string Key { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int MinPlayerCount { get; set; }

		public int MaxPlayerCount { get; set; }

		public LocalizationDictionary LocalizationDictionary { get; set; }

	}
}
