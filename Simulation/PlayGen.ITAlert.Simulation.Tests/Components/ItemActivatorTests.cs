﻿using System;
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
	public class ItemActivatorTests : ComponentTestBase<ItemActivator>
	{
		[Test]
		public void WriteMoreTests()
		{
			Assert.Inconclusive("This class is inadequately tested");
		}

		protected override void TestComponentCreationViaArchetype_PostCreate(ItemActivator component)
		{
			Assert.That(component.Item, Is.Not.Null);
			Assert.That(component.Item.Item.HasValue, Is.False);
		}

	}
}
