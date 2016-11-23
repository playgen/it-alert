using NUnit.Framework;

namespace PlayGen.ITAlert.Simulation.Common.Tests
{
	[TestFixture]
	public class VertexDirectionTests
	{
		[TestCase(EdgeDirection.North, EdgeDirection.South)]
		[TestCase(EdgeDirection.South, EdgeDirection.North)]
		[TestCase(EdgeDirection.West, EdgeDirection.East)]
		[TestCase(EdgeDirection.East, EdgeDirection.West)]
		public void TestComplmentaryCalculation(EdgeDirection source, EdgeDirection complement)
		{
			Assert.AreEqual(complement, source.OppositePosition());
		}
	}
}
