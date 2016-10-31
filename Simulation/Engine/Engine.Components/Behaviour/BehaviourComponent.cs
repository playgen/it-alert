using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components.Behaviour
{
	public abstract class BehaviourComponent : ComponentBase, IBehaviourComponent
	{
		protected BehaviourComponent(IComponentContainer container) 
			: base(container)
		{
		}
	}
}
