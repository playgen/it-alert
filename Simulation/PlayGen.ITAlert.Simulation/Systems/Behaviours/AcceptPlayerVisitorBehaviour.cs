using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Components.Behaviour;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Systems.Messages;
using PlayGen.ITAlert.Simulation.Visitors.Actors;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	[ComponentUsage(typeof(INode))]
	[ComponentDependency(typeof(VisitableNodeBehaviour))]
	public class AcceptPlayerVisitorBehaviour : EntityBehaviourComponent
	{
		private readonly VisitableNodeBehaviour _visitableNode;

		public AcceptPlayerVisitorBehaviour(IEntity container) 
			: base(container)
		{
			_visitableNode = container.GetComponent<VisitableNodeBehaviour>();

			Observer.AddSubscription<VisitorEnteringNodeMessage>(VisitorEnteringNode);

		}

		#region message handlers

		private void VisitorEnteringNode(VisitorEnteringNodeMessage visitorEnteringNodeMessage)
		{
			var player = visitorEnteringNodeMessage.Visitor as Player;
			if (player != null)
			{
				_visitableNode.AddVisitor(visitorEnteringNodeMessage.Visitor, position, offset);

				//OnPlayerEnterDestination(player);
			}
			_visitableNode.AddVisitor(visitorEnteringNodeMessage.Visitor, visitorEnteringNodeMessage.Source, visitorEnteringNodeMessage.MovementOverflow);
		}

		#endregion
	}
}
