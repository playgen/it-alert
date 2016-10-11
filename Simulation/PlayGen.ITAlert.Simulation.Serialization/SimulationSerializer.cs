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
		private readonly EntityRegistrySerializer _serializer;

		public SimulationSerializer()
		{
			_serializer = new EntityRegistrySerializer();
		}

		public byte[] SerializeSimulation(Simulation simulation)
		{
			return EntityRegistrySerializer.Serialize(simulation);
		}

		public Simulation DeserializeSimulation(byte[] simulationBytes)
		{
			return EntityRegistrySerializer.Deserialize<Simulation>(simulationBytes);
		}

		public byte[] SerializeDifferential(Simulation simulation)
		{
			return _serializer.SerializeDifferential<Simulation>(simulation);
		}
		
		public void DeserializeDifferential(byte[] simulationBytes, Simulation simulation)
		{
			_serializer.DeserializeDifferential<Simulation>(simulationBytes, simulation);
		}

	}

}
