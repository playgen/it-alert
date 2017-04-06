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
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Archetypes;

namespace PlayGen.ITAlert.Simulation.Tests.Archetypes
{
	[TestFixture()]
	public class AnalysisTests
	{
		[Test]
		public void TestComponentCreationViaArchetype()
		{
			var configuration = new ECSConfiguration(new List<Archetype>(){ AntivirusWorkstation.Archetype }, null, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Entity entity;
			ecs.TryCreateEntityFromArchetype(AntivirusWorkstation.Archetype, out entity);

		}

		[Test]
		public void WriteMoreTests()
		{
			Assert.Inconclusive("This class is inadequately tested");
		}

	}
}
