﻿using System;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class EdgeConfig : IEquatable<EdgeConfig>
	{
		public int EntityId { get; set; }

		public int Source { get; }
		public EdgeDirection SourcePosition { get; }

		public int Destination { get; }
		//public EdgeDirection DestinationPosition { get; }

		public int Weight { get; }

		// TODO: calculate in graph layout pass
		public int Length { get; set; }

		public string ArchetypeName { get; set; }

		public EdgeConfig(int source, EdgeDirection sourcePosition, int destination, int weight = 1, int length = 1){
			Source = source;
			SourcePosition = sourcePosition;
			Destination = destination;
			Weight = weight;
			Length = length;
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