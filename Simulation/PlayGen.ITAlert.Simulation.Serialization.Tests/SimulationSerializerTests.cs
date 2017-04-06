using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Serialization;
using NUnit.Framework;
using PlayGen.ITAlert.Simulation.Startup;

namespace PlayGen.ITAlert.Simulation.Serialization.Tests
{
	[TestFixture]
	public class SimulationSerializerTests
	{

		[Test]
		public void TestSerializationRountrip()
		{

			var originalSimulation = SimulationHelper.GenerateSimulation(2, 2, 2, 2, 1, new List<Archetype>() {});

			throw new NotImplementedException();
			//var simBytes = serializer.SerializeSimulation(originalSimulation);

			//Assert.That(simBytes.Length, Is.GreaterThan(0));

			//var copySimulation = serializer.DeserializeSimulation(simBytes);

			//Assert.That(copySimulation, Is.Not.Null);

			//var copiedCopy = serializer.SerializeSimulation(copySimulation);

			//Assert.That(copiedCopy, Is.EquivalentTo(simBytes));
		}

		[Test]
		public void TestCompression()
		{
			var random = new Random();
			var randomBytes = new byte[9999];
			random.NextBytes(randomBytes);

			var compressedRandomBytes = CompressionUtil.Compress(randomBytes);

			Assert.False(compressedRandomBytes.SequenceEqual(randomBytes));

			var decompressedRandomBytes = CompressionUtil.Decompress(compressedRandomBytes);

			Assert.True(decompressedRandomBytes.SequenceEqual(randomBytes));
		}

		[Test]
		public void TestMoveIntentSerialization()
		{
			
			var originalSimulation = SimulationHelper.GenerateSimulation(2, 2, 1, 0, 1, new List<Archetype>());

			throw new NotImplementedException();
			//var player = originalSimulation.Players.Single();
			//var destination = originalSimulation.Systems.First(s => s.Id != player.CurrentNode.Id);

			//originalSimulation.RequestMovePlayer(player.Id, destination.Id);

			//Assert.That(player.Intents.Count, Is.EqualTo(1));
			//Assert.That(player.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));
			//Assert.That((player.Intents.Peek() as MoveIntent).Destination.Id, Is.EqualTo(destination.Id));

			//var serializer = new SimulationSerializer();
			//var simBytes = serializer.SerializeSimulation(originalSimulation);
			//var copySimulation = serializer.DeserializeSimulation(simBytes);

			//player = copySimulation.Players.Single();

			//Assert.That(player.Intents.Count, Is.EqualTo(1));
			//Assert.That(player.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));
			//Assert.That((player.Intents.Peek() as MoveIntent).Destination.Id, Is.EqualTo(destination.Id));
		}

		[Test]
		public void TestInfectIntentSerialization()
		{
			var originalSimulation = SimulationHelper.GenerateSimulation(2, 2, 1, 0, 1, new List<Archetype>());
			throw new NotImplementedException();
			//originalSimulation.SpawnVirus(1);

			//var subsystem = originalSimulation.GetEntityById<Subsystem>(originalSimulation.SystemsByLogicalId[1].Id);
			//Assert.That(subsystem.IsInfected, Is.True);

			//var infectionId = (subsystem.GetState() as SystemState).Infection;
			//Assert.That(infectionId.HasValue);

			//var virus = originalSimulation.GetEntityById<Virus>(infectionId.Value);
			//Assert.That(virus, Is.Not.Null);

			//for (var i = 0; i < 121; i++)
			//{
			//	originalSimulation.Tick();
			//}

			//subsystem = originalSimulation.GetEntityById<Subsystem>(originalSimulation.SystemsByLogicalId[1].Id);
			//Assert.That(subsystem.IsInfected, Is.True);
			//infectionId = (subsystem.GetState() as SystemState).Infection;
			//Assert.That(infectionId.HasValue);
			//virus = originalSimulation.GetEntityById<Virus>(infectionId.Value);
			//Assert.That(virus, Is.Not.Null);

			//var newVirus = originalSimulation.GetEntityById<Virus>(infectionId.Value + 1);

			//Assert.That(newVirus, Is.Not.Null);

			//Assert.That(newVirus.Intents.Count, Is.EqualTo(2));
			//Assert.That(newVirus.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));

			//// copy
			//var serializer = new SimulationSerializer();

			//var simBytes = serializer.SerializeSimulation(originalSimulation);
			//var copySimulation = serializer.DeserializeSimulation(simBytes);

			//newVirus = copySimulation.GetEntityById<Virus>(infectionId.Value + 1);

			//Assert.That(newVirus, Is.Not.Null);

			//Assert.That(newVirus.Intents.Count, Is.EqualTo(2));
			////newVirus.Intents.Pop(); // hack to prove stack reversal
			//Assert.That(newVirus.Intents.Peek(), Is.InstanceOf(typeof(MoveIntent)));

		}
	}
}
