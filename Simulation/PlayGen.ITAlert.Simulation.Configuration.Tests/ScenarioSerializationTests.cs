using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using Engine.Serialization;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.Tutorial;

namespace PlayGen.ITAlert.Simulation.Configuration.Tests
{
	[TestFixture]
	public class ScenarioSerializationTests
	{
		[Test]
		public void TestSerializeScenario()
		{
			var scenario = Tutorial1_Movement.Scenario;

			var json = ConfigurationSerializer.SerializeScenario(scenario);

			// TODO: this will fail because of the use of generic func<T> which cannot currently be deseiralized
			// this can be replaced by some types implementing IPlayerConfigFactory, but this rather restricts the signature and lightweightness of the configuration
			// thid might not be such an issue with reference preservation in the serializer
			var scenarioCopy = ConfigurationSerializer.DeserializeScenario<SimulationScenario>(json);
		}

	}
}
