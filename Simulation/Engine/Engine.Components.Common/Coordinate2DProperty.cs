using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Common;
using Engine.Components.Property;
using Engine.Entities;

namespace Engine.Components.Common
{
	public class CoordinateState
	{
		public int X { get; }
		public int Y { get; }

		public CoordinateState(int x, int y)
		{
			X = x;
			Y = y;
		}
	}

	public class Coordinate2DProperty : Property<Vector>, IEmitState
	{
		public object GetState()
		{
			return new CoordinateState(Value.X, Value.Y);
		}
	}
}
