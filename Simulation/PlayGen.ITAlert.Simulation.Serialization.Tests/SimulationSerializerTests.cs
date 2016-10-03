using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.World;
using PlayGen.ITAlert.TestData;

namespace PlayGen.ITAlert.Simulation.Serialization.Tests
{
	[TestFixture]
	public class SimulationSerializerTests
	{

		[Test]
		public void TestSerializationRountrip()
		{
			var originalSimulation = ConfigHelper.GenerateSimulation(2, 2, 2, 2, 1);

			var simBytes = SimulationSerializer.SerializeSimulation(originalSimulation);

			Assert.That(simBytes.Length, Is.GreaterThan(0));

			var copySimulation = SimulationSerializer.DeserializeSimulation(simBytes);

			Assert.That(copySimulation, Is.Not.Null);

			var copiedCopy = SimulationSerializer.SerializeSimulation(copySimulation);

			Assert.That(copiedCopy, Is.EquivalentTo(simBytes));
		}

		[Test]
		public void TestCompression()
		{
			var random = new Random();
			var randomBytes = new byte[9999];
			random.NextBytes(randomBytes);

			var compressedRandomBytes = SimulationSerializer.Compress(randomBytes);

			Assert.False(compressedRandomBytes.SequenceEqual(randomBytes));

			var decompressedRandomBytes = SimulationSerializer.Decompress(compressedRandomBytes);

			Assert.True(decompressedRandomBytes.SequenceEqual(randomBytes));
		}

		[Test]
		public void TestMoveIntentSerialization()
		{
			var originalSimulation = ConfigHelper.GenerateSimulation(2, 2, 1, 0, 1);

			var player = originalSimulation.Players.Single();
			var destination = originalSimulation.Subsystems.First(s => s.Id != player.CurrentNode.Id);

			originalSimulation.RequestMovePlayer(player.Id, destination.Id);

			Assert.That(player.Intents.Count, Is.EqualTo(1));
			Assert.That(player.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));
			Assert.That((player.Intents.Peek() as MoveIntent).Destination.Id, Is.EqualTo(destination.Id));

			var simBytes = SimulationSerializer.SerializeSimulation(originalSimulation);
			var copySimulation = SimulationSerializer.DeserializeSimulation(simBytes);

			player = copySimulation.Players.Single();

			Assert.That(player.Intents.Count, Is.EqualTo(1));
			Assert.That(player.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));
			Assert.That((player.Intents.Peek() as MoveIntent).Destination.Id, Is.EqualTo(destination.Id));
		}

		[Test]
		public void TestInfectIntentSerialization()
		{
			var originalSimulation = ConfigHelper.GenerateSimulation(2, 2, 1, 0, 1);
			originalSimulation.SpawnVirus(1);

			var subsystem = originalSimulation.GetEntityById<Subsystem>(originalSimulation.SubsystemsByLogicalId[1].Id);
			Assert.That(subsystem.IsInfected, Is.True);

			var infectionId = (subsystem.GetState() as SubsystemState).Infection;
			Assert.That(infectionId.HasValue);

			var virus = originalSimulation.GetEntityById<Virus>(infectionId.Value);
			Assert.That(virus, Is.Not.Null);

			for (var i = 0; i < 121; i++)
			{
				originalSimulation.Tick();
			}

			subsystem = originalSimulation.GetEntityById<Subsystem>(originalSimulation.SubsystemsByLogicalId[1].Id);
			Assert.That(subsystem.IsInfected, Is.True);
			infectionId = (subsystem.GetState() as SubsystemState).Infection;
			Assert.That(infectionId.HasValue);
			virus = originalSimulation.GetEntityById<Virus>(infectionId.Value);
			Assert.That(virus, Is.Not.Null);

			var newVirus = originalSimulation.GetEntityById<Virus>(infectionId.Value + 1);

			Assert.That(newVirus, Is.Not.Null);

			Assert.That(newVirus.Intents.Count, Is.EqualTo(2));
			Assert.That(newVirus.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));

			// copy
			var simBytes = SimulationSerializer.SerializeSimulation(originalSimulation);
			var copySimulation = SimulationSerializer.DeserializeSimulation(simBytes);

			newVirus = copySimulation.GetEntityById<Virus>(infectionId.Value + 1);

			Assert.That(newVirus, Is.Not.Null);

			Assert.That(newVirus.Intents.Count, Is.EqualTo(2));
			//newVirus.Intents.Pop(); // hack to prove stack reversal
			Assert.That(newVirus.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));

		}
	}
}
