using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Unity.GameStates.Menu.ScenarioList
{
	public class ScenarioInfo
	{
		public string Name { get; set; }

		public int MinPlayerCount { get; set; }

		public int MaxPlayerCount { get; set; }

		public string Description { get; set; }

		public ScenarioInfo(string n, int min, int max, string desc)
		{
			Name = n;
			MinPlayerCount = min;
			MaxPlayerCount = max;
			Description = desc;
		}
	}
}
