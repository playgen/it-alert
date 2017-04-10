using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;
using Engine.Testing;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Components;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Systems;

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
				new SystemConfiguration<AntivirusEnhancementSystem>()
			};

			var configuration = new ECSConfiguration(archetypes, systems, null);
			var scenario = new TestScenario() {Configuration = configuration};
			var ecs = TestInstaller.CreatTestRoot(scenario).ECS;

			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("Test", out entity));

			var component = entity.GetComponent<ItemStorage>();
			Assert.That(component, Is.Not.Null);
			Assert.That(component.Items.Length, Is.EqualTo(SimulationConstants.SubsystemMaxItems));
			Assert.That(component.Items[AntivirusEnhancementSystem.AnalysisTargetStorageLocation], Is.Not.Null);
		}
	}
}
