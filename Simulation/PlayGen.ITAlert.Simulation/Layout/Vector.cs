using System;
using PlayGen.Engine.Serialization;

namespace PlayGen.ITAlert.Simulation.Layout
{
	public struct Vector : IEquatable<Vector>
	{
		public static Vector PlusX = new Vector(1, 0);
		public static Vector PlusY = new Vector(0, 1);
		public static Vector MinusX = new Vector(-1, 0);
		public static Vector MinusY = new Vector(0, -1);

		[SyncState(StateLevel.Differential)]
		public int X { get; set; }
		[SyncState(StateLevel.Differential)]
		public int Y { get; set; }

		public Vector(int x, int y)
		{
			X = x;
			Y = y;
		}

		public bool Equals(Vector other)
		{
			return X == other.X && Y == other.Y;
		}

		public static Vector operator +(Vector arg1, Vector arg2)
		{
			return new Vector(arg1.X + arg2.X, arg1.Y + arg2.Y);
		}

		public static Vector operator -(Vector arg1, Vector arg2)
		{
			return new Vector(arg1.X - arg2.X, arg1.Y - arg2.Y);
		}
	}
}
