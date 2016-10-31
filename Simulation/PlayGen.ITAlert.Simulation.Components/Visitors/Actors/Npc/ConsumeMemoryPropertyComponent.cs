using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Behaviour;
using PlayGen.ITAlert.Simulation.Entities.Interfaces;
using PlayGen.ITAlert.Simulation.Entities.Visitors;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Components.Visitors.Actors.Npc
{
	public class EmitConsumeMemoryBehviourBehaviourComponent : VisitorBehaviourComponent
	{ 
		public EmitConsumeMemoryBehviourBehaviourComponent(IComponentContainer container) 
			: base(container)
		{
		}

		public override void OnEnterNode(INode current)
		{
			base.OnEnterNode(current);


		}
	}
}

