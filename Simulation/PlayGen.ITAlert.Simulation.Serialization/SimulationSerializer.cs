using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Serialization;

namespace PlayGen.ITAlert.Simulation.Serialization
{
	public class SimulationSerializer
	{
		private readonly ECSSerializer _serializer;

		public SimulationSerializer()
		{
			_serializer = new ECSSerializer();
		}

	public byte[] SerializeSimulation(Simulation simulation)
	{
		return ECSSerializer.Serialize(simulation);
	}

	public Simulation DeserializeSimulation(byte[] simulationBytes)
	{
		return null; // ECSSerializer.Deserialize(simulationBytes);
	}

	public byte[] SerializeDifferential(Simulation simulation)
	{
		return _serializer.SerializeDifferential(simulation);
	}

	public void DeserializeDifferential(byte[] simulationBytes, Simulation simulation)
	{
		_serializer.DeserializeDifferential(simulationBytes, simulation);
	}

}

}
