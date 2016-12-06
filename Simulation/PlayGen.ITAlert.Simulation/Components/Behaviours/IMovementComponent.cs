using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	public interface IMovementComponent : IComponent
	{
		void MoveVisitors(int currentTick);

		void AddVisitor(Entity visitor);

		void AddVisitor(Entity visitor, Entity source, int initialPosition, int currentTick);
	}
}
