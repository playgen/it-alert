using System.Text;
using Newtonsoft.Json;
using PlayGen.ITAlert.Simulation.Serialization;

namespace PlayGen.ITAlert.Photon.Serialization
{
    public static class Serializer
    {
        public static byte[] Serialize(object content)
        {
            var serialziedString = JsonConvert.SerializeObject(content,
                Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            var bytes = Encoding.UTF8.GetBytes(serialziedString);
            var compressedBytes = SimulationSerializer.Compress(bytes);
            return compressedBytes;
        }

        public static T Deserialize<T>(byte[] compressedBytes)
        {
            var decompressedBytes = SimulationSerializer.Decompress(compressedBytes);
            var serializedString = Encoding.UTF8.GetString(decompressedBytes);
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

            var bytes = SimulationSerializer.SerializeSimulation(simulation);
            var compressedBytes = SimulationSerializer.Compress(bytes);
            return compressedBytes;
        }

        public static Simulation.Simulation DeserializeSimulation(byte[] compressedBytes)
        {
            var decompressedBytes = SimulationSerializer.Decompress(compressedBytes);
            var simulation = SimulationSerializer.DeserializeSimulation(decompressedBytes);
            return simulation;
        }
    }
}
