using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Scenario.Configuration
{
	public class StartingLocationSequencePlayerConfigFactory : IPlayerConfigFactory
	{
		private readonly string _archetype;

		private readonly int[] _sequence;

		public StartingLocationSequencePlayerConfigFactory(string archetype, int[] sequence)
		{
			_archetype = archetype;
			_sequence = sequence;
		}

		public PlayerConfig GetNextPlayerConfig(int index)
		{
			var startingLocation = _sequence[index % _sequence.Length];

			return new PlayerConfig()
			{
				ArchetypeName = _archetype,
				Id = index,
				StartingLocation = startingLocation,
			};
		}
	}
}
