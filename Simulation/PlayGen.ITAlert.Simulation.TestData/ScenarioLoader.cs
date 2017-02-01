using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public class ScenarioLoader
	{
		private readonly Scenario[] _scenarios = new[]
		{
			new Scenario()
			{
				Name = "Introduction",
				MinPlayers = 1,
				MaxPlayers = 1,
				
			},
			new Scenario()
			{

			},
			new Scenario()
			{

			},
		};

		public IEnumerable<Scenario> GetScenarios()
		{
			return _scenarios;
		}

	}
}
