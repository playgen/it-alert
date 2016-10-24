using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.Engine.Components.Behaviour;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Entities.Visitors;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Components.Visitors
{
	public abstract class VisitorBehaviourComponentBase : BehaviourComponentBase, IVisitorComponent
	{
		[SyncState(StateLevel.Differential)]
		private int _enterCurrentNodeTick;

		protected VisitorBehaviourComponentBase(IComponentContainer container) 
			: base(container)
		{
		}

		public virtual void OnEnterNode(INode current)
		{
			_enterCurrentNodeTick = 0;
		}

		public virtual void OnExitNode(INode current)
		{
		}

		public virtual void OnTick(INode currentNode)
		{
		}
	}
}
