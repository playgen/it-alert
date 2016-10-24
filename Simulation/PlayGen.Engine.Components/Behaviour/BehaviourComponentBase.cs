using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.Engine.Components.Behaviour
{
	public abstract class BehaviourComponentBase : ComponentBase, IBehaviourComponent
	{
		protected BehaviourComponentBase(IComponentContainer container) 
			: base(container)
		{
		}
	}
}
