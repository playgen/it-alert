using System;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public class NodeConfig : EntityConfig, IEquatable<NodeConfig>
	{
		public int X { get; set; }
		public int Y { get; set; }
		public string Name { get; set; }

		public NodeConfig()
		{
			
		}

		public NodeConfig(int x, int y, string archetypeName, string name = null)
		{
			X = x;
			Y = y;
			Name = name ?? $"{x}{y}";
			Archetype = archetypeName;
		}

		public bool Equals(NodeConfig other)
		{
			return Id == other?.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return Id.ToString();
		}
	}
}
