using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
	[ComponentDependency(typeof(Visitors))]
	public class AcceptPlayerVisitorBehaviour : Component
	{
		private readonly Visitors _visitableNode;

		public AcceptPlayerVisitorBehaviour(IEntity entity) 
			: base(entity)
		{
			_visitableNode = entity.GetComponent<Visitors>();

			AddSubscription<VisitorEnteringNodeMessage>(VisitorEnteringNode);

		}

		#region message handlers

		private void VisitorEnteringNode(VisitorEnteringNodeMessage visitorEnteringNodeMessage)
		{
			var player = visitorEnteringNodeMessage.Visitor as Player;
			if (player != null)
			{
				//_visitableNode.AddVisitor(visitorEnteringNodeMessage.Visitor, position, offset);

				//OnPlayerEnterDestination(player);
			}
			_visitableNode.AddVisitor(visitorEnteringNodeMessage.Visitor, visitorEnteringNodeMessage.Source, visitorEnteringNodeMessage.MovementOverflow);
		}

		#endregion
	}
}
