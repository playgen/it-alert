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
		public IPlayerConfigFactory PlayerConfigFactory { get; set; }

		/// <summary>
		/// Dictionary keyed by country-code, then by text key
		/// </summary>
		public LocalizationDictionary LocalizationDictionary { get; set; }

	}
}
