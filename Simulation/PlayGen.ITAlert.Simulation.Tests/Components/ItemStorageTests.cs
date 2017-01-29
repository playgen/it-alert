using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Testing;
using Engine.Testing.Components;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Tests.Components
{
	[TestFixture]
	public class ItemStorageTests : ComponentTestBase<ItemStorage>
	{
		[Test]
		public void WriteMoreTests()
		{
			Assert.Inconclusive("This class is inadequately tested");
		}

		protected override void TestComponentCreationViaArchetype_PostCreate(ItemStorage component)
		{
			Assert.That(component.Items, Is.Not.Null);
			Assert.That(component.Items[0], Is.Null);
		}

	}
}
