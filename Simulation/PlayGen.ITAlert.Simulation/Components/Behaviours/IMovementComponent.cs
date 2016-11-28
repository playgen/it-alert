using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Core.Components;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	public interface IMovementComponent : IComponent
	{
		void MoveVisitors(int currentTick);

		void AddVisitor(IEntity visitor);

		void AddVisitor(IEntity visitor, IEntity source, int initialPosition, int currentTick);
	}
}
