using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Properties
{
	public sealed class EndToEndMovement : FlagComponent, IMovementType
	{
		public EndToEndMovement(IEntity entity) : base(entity)
		{
		}
	}
}
