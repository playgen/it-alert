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

					Intent visitorIntent;
					if (visitorIntents != null && visitorIntents.TryPeek(out visitorIntent))
					{
						var moveIntent = visitorIntent as MoveIntent;
						if (moveIntent != null)
						{
							
						}
					}

					switch (movementMethod)
					{
						case MovementMethod.Continuous:

						
					}
				}
			}
		}

		private void MoveContinuous()
		{
			if (destination != null && destination != this)
			{
				//TODO: refactor to susbsytem onenter
				Connection exitConnection;
				if (!ExitRoutes.TryGetValue(destination.Id, out exitConnection))
				{
					throw new Exception("Route finding is broken!");
				}
				// exist selection must be deterministic to keep simulations in sync, use the current tick
				var exitPosition = ExitNodePositions[exitConnection.Id].Direction.ToPosition(Positions);
				var nextPosition = (visitorPosition.Position + actor.Speed) % Positions;

				var exitAfterTop = exitPosition < visitorPosition.Position;
				var nextPositionAfterTop = nextPosition < visitorPosition.Position;
				var nextPositionAfterExit = nextPosition > exitPosition;

				if (visitorPosition.Position == exitPosition
					|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
					|| (exitAfterTop == false & nextPositionAfterExit))
				{
					var overflow = Math.Max(nextPosition - exitPosition, 0);
					VisitorLeaving(actor, exitConnection, overflow);
					return true;
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

		//public override void OnDeserialized()
		//{
		//	foreach (var visitorPosition in VisitorPositions.Values)
		//	{
		//		visitorPosition.Visitor.EntityDestroyed += Visitor_EntityDestroyed;
		//	}
		//}

		private void MoveVisitors()
		{
			foreach (var visitorPosition in VisitorPositionState.ToArray())
			{
				var actor = visitorPosition.Value.Visitor as IActor;
				if (actor != null)
				{
					var position = visitorPosition.Value.Position;
					var nextPosition = (position + actor.MovementSpeed);

					if (position == (Weight - 1) || nextPosition >= Weight)
					{
						var overflow = Math.Max(nextPosition - Weight, 0);
						VisitorLeaving(actor, Tail, overflow);
						continue;
					}
					visitorPosition.Value.UpdatePosition(nextPosition, CurrentTick);
				}
			}
		}

		#endregion

		#region systems

		/// <summary>
		/// Handle behaviour for a player on tick
		/// </summary>
		/// <param name="player"></param>
		/// <param name="visitorPosition"></param>
		private void OnTickPlayer(Player player, VisitorPosition visitorPosition)
		{
			Intent intent;
			if (player.TryGetIntent(out intent))
			{
				var moveIntent = intent as MoveIntent;
				if (moveIntent != null)
				{
					if (HandleMoveIntent(player, moveIntent, visitorPosition))
					{
						return;
					}
				}

			}
			// update player position
			// visitorPosition.UpdatePosition((visitorPosition.Position + player.MovementSpeed) % _visitableNode.Positions, CurrentTick);
		}

		/// <summary>
		/// Handle behaviour for a virus on tick
		/// </summary>
		/// <param name="virus"></param>
		private void OnTickVirus(Virus virus)
		{

		}

		private void OnTickItem(IItem item)
		{
			// nothing to do here
		}

		private bool HandleMoveIntent(IActor actor, MoveIntent intent, VisitorPosition visitorPosition)
		{
			//// this should have already been tested
			//var destination = intent.Destination as System;
			//if (destination != null && destination != this)
			//{
			//	//TODO: refactor to susbsytem onenter
			//	Connection exitConnection;
			//	if (!ExitRoutes.TryGetValue(destination.Id, out exitConnection))
			//	{
			//		throw new Exception("Route finding is broken!");
			//	}
			//	// exist selection must be deterministic to keep simulations in sync, use the current tick
			//	var exitPosition = ExitNodePositions[exitConnection.Id].Direction.ToPosition(Positions);
			//	var nextPosition = (visitorPosition.Position + actor.MovementSpeed) % Positions;

			//	var exitAfterTop = exitPosition < visitorPosition.Position;
			//	var nextPositionAfterTop = nextPosition < visitorPosition.Position;
			//	var nextPositionAfterExit = nextPosition > exitPosition;

			//	if (visitorPosition.Position == exitPosition
			//		|| (exitAfterTop && nextPositionAfterTop && nextPositionAfterExit)
			//		|| (exitAfterTop == false & nextPositionAfterExit))
			//	{
			//		var overflow = Math.Max(nextPosition - exitPosition, 0);
			//		VisitorLeaving(actor, exitConnection, overflow);
			//		return true;
			//	}
			//}
			return false;
		}

		#endregion
	}
}
