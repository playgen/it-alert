using System;

namespace PlayGen.ITAlert.Common
{
	public static class VertexDirectionExtensions
	{
		public static VertexDirection OppositePosition(this VertexDirection position)
		{
			return (VertexDirection)(((int)position + 2) % 4);
		}


		public static int ToPosition(this VertexDirection direction, int positions)
		{
			switch (direction)
			{
				case VertexDirection.Top:
					return 0;
				case VertexDirection.Right:
					return positions / 4;
				case VertexDirection.Bottom:
					return positions / 2;
				case VertexDirection.Left:
					return (3 * positions) / 4;;
				default:
					throw new Exception("Invalid position");
			}
		}

		public static int PositionsToExit(this VertexDirection direction, VertexDirection exit)
		{
			return (4 + ((int) exit - (int) direction))%4;
		}
	}
}
