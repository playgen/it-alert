using System;

namespace PlayGen.ITAlert.Simulation.Common
{
	public static class VertexDirectionExtensions
	{
		const int EdgeDirectionMax = 2 * (int) EdgeDirection.South;

		public static EdgeDirection OppositePosition(this EdgeDirection position)
		{
			return (EdgeDirection)(((int)position + (int)EdgeDirection.South) % EdgeDirectionMax);
		}


		public static int ToPosition(this EdgeDirection direction, int positions)
		{
			switch (direction)
			{
				case EdgeDirection.North:
					return 0;
				case EdgeDirection.East:
					return positions / 4;
				case EdgeDirection.South:
					return positions / 2;
				case EdgeDirection.West:
					return (3 * positions) / 4;;
				default:
					throw new Exception("Invalid position");
			}
		}

		public static EdgeDirection FromPosition(this int position, int positions)
		{
			if (position == 0)
			{
				return EdgeDirection.North;
			}
			if (position == positions/4)
			{
				return EdgeDirection.East;
			}
			if (position == positions / 2)
			{
				return EdgeDirection.South;
			}
			if (position == (3 * positions) / 4)
			{
				return EdgeDirection.West;
			}
			throw new Exception("Position was not an exact EdgeDirection");
		}

		public static int PositionsToExit(this EdgeDirection direction, EdgeDirection exit)
		{
			return (EdgeDirectionMax + ((int) exit - (int) direction)) % EdgeDirectionMax;
		}
	}
}
