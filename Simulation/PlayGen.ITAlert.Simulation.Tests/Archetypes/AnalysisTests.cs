﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Testing;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace PlayGen.ITAlert.Simulation.Tests.Archetypes
{
	[TestFixture()]
	public class AnalysisTests
	{
		[Test]
		public void TestComponentCreationViaArchetype()
		{
			var configuration = new ECSConfiguration(new List<Archetype>(){ GameEntities.AnalysisEnhancement }, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			ecs.CreateEntityFromArchetype(GameEntities.AnalysisEnhancement.Name);

		}

		[Test]
		public void WriteMoreTests()
		{
			Assert.Inconclusive("This class is inadequately tested");
		}

	}
}