using System.Text;
using Newtonsoft.Json;
using PlayGen.ITAlert.Simulation.Serialization;

namespace PlayGen.ITAlert.Photon.Serialization
{
	public static class Serializer
	{
		private static SimulationSerializer _simulationSerializer = new SimulationSerializer();

		public static byte[] Serialize(object content)
		{
			var serialziedString = JsonConvert.SerializeObject(content,
				Formatting.None,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All
				});

			var bytes = Encoding.UTF8.GetBytes(serialziedString);
			return bytes;
		}

		public static T Deserialize<T>(byte[] bytes)
		{
			var serializedString = Encoding.UTF8.GetString(bytes);
			var deserializedObject = JsonConvert.DeserializeObject<T>(serializedString,
				new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				});

			return deserializedObject;
		}

		public static byte[] SerializeSimulation(object simulationObject)
		{
			var simulation = (Simulation.Simulation) simulationObject;

			var bytes = _simulationSerializer.SerializeSimulation(simulation);
			return bytes;
		}

		public static Simulation.Simulation DeserializeSimulation(byte[] bytes)
		{
			var simulation = _simulationSerializer.DeserializeSimulation(bytes);
			return simulation;
		}
	}
}
