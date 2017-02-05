using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;
using Engine.Testing;
using NUnit.Framework.Internal;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Tests.Systems
{
	[TestFixture]
	public class ItemStorageSystemTests
	{
		[Test]
		public void WriteMoreTests()
		{
			Assert.Inconclusive("This class is inadequately tested");
		}

		[Test]
		public void TestSystemOnNewEntityContainerInitialization()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("Test")
				{
					Components =
					{
						new ComponentBinding<ItemStorage>(),
					}
				},
			};

			var systems = new List<SystemConfiguration>()
			{
				new SystemConfiguration<ItemStorageSystem>()
			};

			var configuration = new ECSConfiguration(archetypes, systems, null);
			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;
			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("Test", out entity));

			var component = entity.GetComponent<ItemStorage>();
			Assert.That(component, Is.Not.Null);
			Assert.That(component.Items.Length, Is.EqualTo(SimulationConstants.SubsystemMaxItems));
			Assert.That(component.Items[0], Is.Not.Null);
		}
	}
}
