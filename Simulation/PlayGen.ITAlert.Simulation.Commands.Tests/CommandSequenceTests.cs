using System.Linq;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.TestData;

namespace PlayGen.ITAlert.Simulation.Commands.Tests
{
    [TestFixture]
    public class CommandSequenceTests
    {
        [TestCase(15, 23, 400)]
        [TestCase(50, 100, 1000)]
        [TestCase(1123, 953, 15032)]
        [TestCase(1, 5, 345)]
        public void CreateCommandSequence(int startTick, int tickInterval, int endTick)
        {
            var subsystemLogicalIds = Enumerable.Range(1, 5).ToList();

            var commandSequence = CommandSequenceHelper.GenerateCommandSequence(subsystemLogicalIds, startTick, tickInterval, endTick);

            var currentTick = 0;
            while (commandSequence.HasPendingCommands)
            {
                currentTick++;

                var commands = commandSequence.Tick();

                if ((currentTick - startTick)%tickInterval == 0 && startTick <= currentTick)
                {
                    // Should not be null on ticks where commands were set to be generated
                    Assert.NotNull(commands);
                }
                else
                {
                    // Should be null on ticks where commands were not set to be generated
                    Assert.Null(commands);
                }
            }
        }
    }
}
