using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Components.Behaviour;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	[ComponentUsage(typeof(INode))]
	[ComponentDependency(typeof(VisitableNodeBehaviour))]
	public class MoveVisitingActorsBehaviour : EntityBehaviourComponent, ITickableComponent
	{
		private readonly VisitableNodeBehaviour _visitableNode;
		
		public MoveVisitingActorsBehaviour(IEntity container) : base(container)
		{
		}

		public void Tick(int currentTick)
		{
			throw new NotImplementedException();
		}
	}
}
