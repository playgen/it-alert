using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components.Property;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
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
			//Weight = Positions * weight * length;
			return 0;
		} 
	}
}
