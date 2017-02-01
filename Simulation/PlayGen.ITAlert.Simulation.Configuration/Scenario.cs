﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class Scenario
	{
		public string Name { get; set; }

		public int MinPlayers { get; set; }

		public int MaxPlayers { get; set; }

		public SimulationConfiguration Configuration { get; set; }
	}
}