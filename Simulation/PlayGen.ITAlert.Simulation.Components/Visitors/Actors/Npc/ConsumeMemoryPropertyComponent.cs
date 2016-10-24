using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Behaviour;

namespace PlayGen.ITAlert.Simulation.Components.Visitors.Actors.Npc
{
	public class EmitConsumeMemortBehviourComponent : BehaviourComponentBase, IVisitorComponent
	{
		public EmitConsumeMemortBehviourComponent(IComponentContainer container) 
			: base(container)
		{
		}
	}
}
