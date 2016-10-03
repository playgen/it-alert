using NUnit.Framework;

namespace PlayGen.ITAlert.Common.Tests
{
	[TestFixture]
	public class VertexDirectionTests
	{
		[TestCase(VertexDirection.Top, VertexDirection.Bottom)]
		[TestCase(VertexDirection.Bottom, VertexDirection.Top)]
		[TestCase(VertexDirection.Left, VertexDirection.Right)]
		[TestCase(VertexDirection.Right, VertexDirection.Left)]
		public void TestComplmentaryCalculation(VertexDirection source, VertexDirection complement)
		{
			Assert.AreEqual(complement, source.OppositePosition());
		}
	}
}
