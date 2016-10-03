using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PlayGen.ITAlert.Common;

namespace PlayGen.ITAlert.Configuration.Tests
{
	[TestFixture]
	public class EdgeConfigTests
	{

		[TestCase(0, 0, 0)]
		[TestCase(1, 0, 256)]
		[TestCase(1, 1, 257)]

		public void TestHashCode(int source, int destination, int code)
		{
			var edge = new EdgeConfig(source, VertexDirection.Bottom, destination);
			Assert.AreEqual(code, edge.GetHashCode());

			var edges = new HashSet<EdgeConfig> {edge};
			Assert.False(edges.Add(edge));
		}

	}
}
