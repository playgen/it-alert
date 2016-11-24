using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Engine.Components;
using Engine.Core.Components;
using Engine.Core.Entities;
using Engine.Entities;
using Engine.Planning;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Properties;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Systems.Messages;
using PlayGen.ITAlert.Simulation.VisitorsProperty;
using PlayGen.ITAlert.Simulation.VisitorsProperty.Actors;
using PlayGen.ITAlert.Simulation.VisitorsProperty.Actors.Intents;

namespace PlayGen.ITAlert.Simulation.Systems
{
	[ComponentDependency(typeof(Visitors))]
	[ComponentDependency(typeof(EntrancePositions))]
	[ComponentDependency(typeof(ExitPositions))]
	[ComponentDependency(typeof(ExitRoutes))]
	[ComponentDependency(typeof(MovementCost))]
	[ComponentDependency(typeof(Movement))]
	public class VisitorMovement : Engine.Components.System
	{
		public VisitorMovement(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{
		}

		public void Tick(int currentTick)
		{
			var nodes = ComponentRegistry.GetComponentEntitesImplmenting<Visitors>();

			foreach (var node in nodes)
			{
				var visitors = node.Component.Value;

				var entrancePositions = node.Entity.GetComponent<EntrancePositions>().Value;
				var exitPositions = node.Entity.GetComponent<ExitPositions>().Value;

				var routes = node.Entity.GetComponent<ExitRoutes>().Value;
				var movementCost = node.Entity.GetComponent<MovementCost>().Value;

				var movementMethod = node.Entity.GetComponent<Movement>().Value;

				foreach (var visitor in visitors.Values)
				{
					var visitorIntents = visitor.Visitor.GetComponent<IntentsProperty>();
					var movementSpeed = visitor.Visitor.GetComponent<MovementSpeed>().Value;

						int? exitPosition = null;

					Intent visitorIntent;
					if (visitorIntents != null && visitorIntents.TryPeek(out visitorIntent))
					{
						var moveIntent = visitorIntent as MoveIntent;
						if (moveIntent != null)
						{
							exitPosition = routes[moveIntent.Destination];
						}
					}

					var nextPosition = visitor.Position + movementSpeed;

					switch (movementMethod)
					{
						case MovementMethod.Linear:

							if (visitor.Position == (movementCost - 1) || nextPosition >= movementCost)
							{
								var overflow = Math.Max(nextPosition - movementCost, 0);

								//VisitorLeaving(actor, Tail, overflow);

								continue;
							}

							break;

						case MovementMethod.Continuous:
							nextPosition %= SimulationConstants.SubsystemPositions;

							if (exitPosition.HasValue)
							{
								var exitAfterTop = exitPosition < visitor.Position;
								var nextPositionAfterTop = nextPosition < visitor.Position;
								var nextPositionAfterExit = nextPosition > exitPosition;

								if (visitor.Position == exitPosition
									|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
									|| (exitAfterTop == false & nextPositionAfterExit))
								{
									var overflow = Math.Max(nextPosition - exitPosition.Value, 0);

									//VisitorLeaving(actor, exitConnection, overflow);
								}
							}
							break;
					}
					visitor.UpdatePosition(nextPosition, currentTick);

				}
			}
		}

		#region connections


		#region visitor message handling



		#endregion

		#region visitor movement methods

		public void AddVisitor(IVisitor visitor, INode source, int overflow)
		{
			// determine entrance position
			var sourceConnection = source as Connection;
			var sourceSystem = sourceConnection == null ? source : sourceConnection.Head;
			var direction = EntranceNodePositions.ContainsKey(sourceSystem.Id) ? EntranceNodePositions[sourceSystem.Id].Direction : EdgeDirection.North;

			var subscriptionsDisposable = new CompositeDisposable();
			// add to visitors
			var visitorPosition = new VisitorPosition(visitor, direction.ToPosition(Positions) + overflow, Entity.CurrentTick, subscriptionsDisposable);
			VisitorPositions.Add(visitor.Id, visitorPosition);

			// big TODO: we have a message loop here this isnt good

			// subscribe to messages from the visitor
			subscriptionsDisposable.Add(visitor.Subscribe(Entity));
			// subscribe the visitor to messages from this node
			subscriptionsDisposable.Add(Entity.Subscribe(visitor));

			var visitorEnteredNodeMessage = new VisitorEnteredNodeMessage(visitor, (INode)Entity);
			// notify the visitor it has entered a node
			visitor.OnNext(visitorEnteredNodeMessage);
			// notify other components on this node that a visitor has entered
			Entity.OnNext(visitorEnteredNodeMessage);
		}

		// TODO: this could be replaced with a disposing message from the visitor
		/// <summary>
		/// handle the entity destroyed event for our visitors as they need to be removed from the collection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Visitor_EntityDestroyed(object sender, EventArgs e)
		{

		}

		#endregion

		protected virtual void VisitorLeaving(IVisitor visitor, INode exitNode, int overflow)
		{
			visitor.EntityDestroyed -= Visitor_EntityDestroyed;

			VisitorPositions[visitor.Id].VisitorSubscription.Dispose();
			VisitorPositions.Remove(visitor.Id);

			//visitor.OnExitNode(Entity);

			// call add visitor on exit node and pass player
			// TODO: exit node visitor entering 
			//exitNode.GetComponent<Visitors>().AddVisitor(visitor, Entity, overflow);
			// TODO: item behaviour should respond to visitor leaving message
			//var player = visitor as Player;
			//if (player != null)
			//{
			//	var playerItem = Items.SingleOrDefault(i => i != null && i.Owner == player);
			//	if (playerItem != null)
			//	{
			//		//GetItem(playerItem);
			//		player.SetItem(playerItem);
			//	}
			//}

			// base.VisitorLeaving(visitor, exitNode, overflow);
		}

		#endregion
	}
}
