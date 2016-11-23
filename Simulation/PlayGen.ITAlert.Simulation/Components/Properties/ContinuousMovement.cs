using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public class ContinuousMovement : FlagComponent, IMovementType
	{
		public ContinuousMovement(IEntity entity) : base(entity)
		{
		}
	}
}
