using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PlayGen.Engine.Serialization;

namespace PlayGen.ITAlert.Simulation.Serialization
{
	public class SimulationSerializer
	{
		public static byte[] SerializeSimulation(Simulation simulation)
		{
			return EntityRegistrySerializer.Serialize(simulation);
		}

		public static Simulation DeserializeSimulation(byte[] simulationBytes)
		{
			return EntityRegistrySerializer.Deserialize<Simulation>(simulationBytes);
		}
	}
}
