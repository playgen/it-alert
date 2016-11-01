using System.Collections.Generic;
using System.IO;
using Engine.Serialization.Tests;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Serialization;
using PlayGen.ITAlert.Simulation.TestData;

namespace PlayGen.ITAlert.Simulation.CLI
{
	public class Program
	{
		private static readonly string DebugPath = Path.Combine(Directory.GetCurrentDirectory(), "output");


		public static void Main(string[] args)
		{
			//TickUntilInfection();
			//SerializationLoop();
			JsonSerializerExperiements.TestReferenceResolution();

			var simulation = ConfigHelper.GenerateSimulation(3, 4, 2, new Dictionary<ItemType, int>()
			{
				{ItemType.Repair, 2},
				{ItemType.Scanner, 2},
				{ItemType.Cleaner, 2},
			}, 1);

			var serializer = new SimulationSerializer();

			var simulationBytes = serializer.SerializeSimulation(simulation);
			File.WriteAllBytes(Path.Combine(DebugPath, $"full.json"), simulationBytes);

			var copySimulation = serializer.DeserializeSimulation(simulationBytes);

			simulation.Tick();

			simulationBytes = serializer.SerializeDifferential(simulation);
			File.WriteAllBytes(Path.Combine(DebugPath, $"diff.json"), simulationBytes);

			serializer.DeserializeDifferential(simulationBytes, copySimulation);

			simulationBytes = serializer.SerializeSimulation(copySimulation);
			File.WriteAllBytes(Path.Combine(DebugPath, $"post-diff.json"), simulationBytes);

		}

		//private static void TickUntilInfection()
		//{
		//	using (var simulation = ConfigHelper.GenerateSimulation(3, 4, 2, new Dictionary<ItemType, int>()
		//	{
		//		{ ItemType.Repair, 2 },
		//		{ ItemType.Scanner, 2 },
		//		{ ItemType.Cleaner, 2 },
		//	}, 1))
		//	{
		//		simulation.SpawnVirus(simulation.Systems.First().Id);

		//		var i = 0;

		//		while (simulation.IsGameFailure == false)
		//		{
		//			var state = SimulationSerializer.SerializeSimulation(simulation);
		//			File.WriteAllBytes(Path.Combine(DebugPath, $"{i++}.json"), state);

		//			var subsystemState = simulation.Systems.Select(ss => ss.GetState());

		//			simulation.Tick();
		//		}
		//	}
		//}

		//private static void SerializationLoop()
		//{
		//	var jsonSettings = new JsonSerializerSettings();
		//	jsonSettings.Converters.Add(new StringEnumConverter());
		//	jsonSettings.TypeNameHandling = TypeNameHandling.Auto;

		//	var stopwatch = new Stopwatch();

		//	long worst = 0;
		//	long sum = 0;

		//	const int repeat = 1;

		//	for (var i = 0; i < repeat; i++)
		//	{
		//		byte[] compressedState;

		//		using (var simulation = ConfigHelper.GenerateSimulation(3, 4, 2, new Dictionary<ItemType, int>()
		//		{
		//			{ ItemType.Repair, 2 },
		//			{ ItemType.Scanner, 2 },
		//			{ ItemType.Cleaner, 2 },
		//		}, 1))
		//		{
		//			stopwatch.Start();
		//			var state = SimulationSerializer.SerializeSimulation(simulation);
		//			stopwatch.Stop();
		//			if (stopwatch.ElapsedTicks > worst)
		//			{
		//				worst = stopwatch.ElapsedTicks;
		//			}
		//			sum += stopwatch.ElapsedTicks;
		//			Console.WriteLine($"Serialize took {stopwatch.ElapsedTicks:N5}.");

		//			File.WriteAllBytes(Path.Combine(DebugPath, $"pre.json"), state);

		//			stopwatch.Reset();
		//			compressedState = EntityRegistrySerializer.Compress(state);
		//			stopwatch.Stop();
		//			Console.WriteLine($"Compress took {stopwatch.ElapsedTicks:N5}.");
		//		}

		//		stopwatch.Reset();
		//		var decompressedState = EntityRegistrySerializer.Decompress(compressedState);
		//		stopwatch.Stop();
		//		Console.WriteLine($"Decompress took {stopwatch.ElapsedTicks:N5}.");
		//		File.WriteAllBytes(Path.Combine(DebugPath, $"post.json"), decompressedState);

		//		stopwatch.Reset();
		//		using (var simulation = SimulationSerializer.DeserializeSimulation(decompressedState))
		//		{
		//			stopwatch.Stop();
		//			Console.WriteLine($"Deserialize took {stopwatch.ElapsedTicks:N5}.");

		//			var subsystemState = simulation.Systems.Select(ss => ss.GetState()).Where(ss => (ss as SystemState).VisitorPositions.Any());

		//		}
		//		Thread.Sleep(100);
		//	}
		//	Console.WriteLine($"Worst {worst}. Average {sum / repeat}");
		//}
	}
}
