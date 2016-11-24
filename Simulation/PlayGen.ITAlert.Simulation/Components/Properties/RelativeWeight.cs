using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components.Property;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class RelativeWeight : ReadOnlyProperty<int>
	{
		protected RelativeWeight(IEntity entity) 
			: base(entity)
		{
			ValueGetter = CalculateWeight;
		}

		private int CalculateWeight()
		{
			//// calculate weight
			//var length = Math.Max(1, Math.Max(Math.Abs(Head.X - Tail.X), Math.Abs(Head.Y - Tail.Y)));
			//RelativeWeight = weight;
			//Weight = ConnectionPositions * weight * length;
			return 0;
		} 
	}
}
