using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Components.Behaviour;
using Engine.Core.Entities;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	[ComponentUsage(typeof(INode))]
	[ComponentDependency(typeof(VisitableNodeBehaviour))]
	public class MoveVisitingActorsBehaviour : Component, ITickableComponent
	{
		private readonly VisitableNodeBehaviour _visitableNode;
		
		public MoveVisitingActorsBehaviour(IEntity entity) 
			: base(entity)
		{
		}

		public void Tick(int currentTick)
		{
			throw new NotImplementedException();
		}

		#region connections


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
			visitorPosition.UpdatePosition((visitorPosition.Position + player.MovementSpeed) % Positions, CurrentTick);
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
			// this should have already been tested
			var destination = intent.Destination as System;
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
				var nextPosition = (visitorPosition.Position + actor.MovementSpeed) % Positions;

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
			return false;
		}

		protected override void VisitorLeaving(IVisitor visitor, INode exitNode, int overflow)
		{
			var player = visitor as Player;
			if (player != null)
			{
				var playerItem = Items.SingleOrDefault(i => i != null && i.Owner == player);
				if (playerItem != null)
				{
					GetItem(playerItem);
					player.SetItem(playerItem);
				}
			}

			base.VisitorLeaving(visitor, exitNode, overflow);
		}


		#endregion
	}
}
