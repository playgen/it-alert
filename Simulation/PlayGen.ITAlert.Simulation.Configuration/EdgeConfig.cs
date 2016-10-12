using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class EdgeConfig : IEquatable<EdgeConfig>
	{
		public int EntityId { get; set; }

		public int Source { get; }
		public VertexDirection SourcePosition { get; }

		public int Destination { get; }
		//public VertexDirection DestinationPosition { get; }

		public int Weight { get; }

		public EdgeConfig(int source, VertexDirection sourcePosition, int destination, int weight = 1){
			Source = source;
			SourcePosition = sourcePosition;
			Destination = destination;
			Weight = weight;
		}

		public bool Equals(EdgeConfig other)
		{
			return Source == other.Source 
				&& Destination == other.Destination 
				&& SourcePosition == other.SourcePosition;
		}

		public override int GetHashCode()
		{
			return (Source << 8) | Destination;
		}

		public override string ToString()
		{
			return $"Edge from {Source}[{SourcePosition}] to {Destination}, Weight {Weight}";
		}
	}
}
