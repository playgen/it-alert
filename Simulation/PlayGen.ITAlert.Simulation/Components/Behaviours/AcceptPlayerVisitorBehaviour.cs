using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Components.Messages;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	[ComponentDependency(typeof(Properties.Visitors))]
	public class AcceptPlayerVisitorBehaviour : Component
	{
		private readonly Properties.Visitors _visitableNode;

		public AcceptPlayerVisitorBehaviour(IEntity entity) 
			: base(entity)
		{
			_visitableNode = entity.GetComponent<Components.Properties.Visitors>();

			AddSubscription<VisitorEnteringNodeMessage>(VisitorEnteringNode);

		}

		#region message handlers

		private void VisitorEnteringNode(VisitorEnteringNodeMessage visitorEnteringNodeMessage)
		{
			//var player = visitorEnteringNodeMessage.Visitor as Player;
			//if (player != null)
			//{
			//	//_visitableNode.AddVisitor(visitorEnteringNodeMessage.Visitor, position, offset);

			//	//OnPlayerEnterDestination(player);
			//}
			//_visitableNode.AddVisitor(visitorEnteringNodeMessage.Visitor, visitorEnteringNodeMessage.Source, visitorEnteringNodeMessage.MovementOverflow);
		}

		#endregion
	}
}
