using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Entities;
using Engine.Testing;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Enhacements;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Systems.Enhancements;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Tests.Systems.Enhancements
{
	[TestFixture()]
	public class AnalyserEnhancementExtensionTests
	{

		[Test]
		public void TestSystemOnNewEntityContainerInitialization()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("Test")
				{
					Components =
					{
						{ typeof(ComponentBinding<ItemStorage>), new ComponentBinding<ItemStorage>() },
						{ typeof(ComponentBinding<Antivirus>), new ComponentBinding<Antivirus>() },
					}
				},
			};

			var systems = new List<SystemConfiguration>()
			{
				new SystemConfiguration<EnhancementSystem>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[]
					{
						new SystemExtensionConfiguration<IEnhancementSystemExtension>()
						{
							Implementations = new SystemExtensionImplementation[]
							{
								new SystemExtensionConfiguration<IEnhancementSystemExtension>.SystemExtensionImplementation<AntivirusEnhancementSystemExtension>(), 
							}
						}
					}
				}
			};

			var configuration = new ECSConfiguration(archetypes, systems, null);
			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("Test", out entity));

			var component = entity.GetComponent<ItemStorage>();
			Assert.That(component, Is.Not.Null);
			Assert.That(component.Items.Length, Is.EqualTo(SimulationConstants.SubsystemMaxItems));
			Assert.That(component.Items[AntivirusEnhancementSystemExtension.AnalysisTargetStorageLocation], Is.Not.Null);
		}
	}
}
