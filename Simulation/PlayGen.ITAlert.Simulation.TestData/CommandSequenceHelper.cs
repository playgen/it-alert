using System;
using System.Collections.Generic;
using Engine.Commands;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Sequence;

namespace PlayGen.ITAlert.Simulation.Startup
{
	public static class CommandSequenceHelper
	{
		private static readonly Random Random = new Random();

		// TODO: this needs totally reimplementing
		public static CommandSequence GenerateCommandSequence(int startTick = 50, int tickInterval = 100, int endTick = 1000)
		{
			var range = endTick - startTick;
			var entryCount = (int)Math.Floor((float)range / tickInterval);
			var commands = new CommandSequenceEntry[entryCount + 1];

			var index = 0;
			for (var tick = startTick; tick <= endTick; tick += tickInterval)
			{
				commands[index] = new CommandSequenceEntry
				{
					Tick = tick,
					Commands = new ICommand[]
					{
						//new SpawnNpcCommand
						//{
						//	SystemLogicalId = subsystemLogicalIds[Random.Next(subsystemLogicalIds.Count)]
						//}
					}
				};

				index++;
			}

			return new CommandSequence(commands);
		}
	}
}
