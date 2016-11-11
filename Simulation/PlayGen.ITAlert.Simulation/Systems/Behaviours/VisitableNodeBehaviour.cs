using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Engine.Components;
using Engine.Components.Behaviour;
using Engine.Components.Property;
using Engine.Core.Entities;
using Engine.Core.Serialization;
using Engine.Entities.Messages;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Layout;
using PlayGen.ITAlert.Simulation.Systems.Messages;
using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	public class VisitorPositionState : Dictionary<int, int>
	{
		 
	}

	[ComponentUsage(typeof(INode))]
	public class VisitableNodeBehaviour : Component, IEmitState<VisitorPositionState>
	{
		private const int MinPositions = 0;
		private const int MaxPositions = int.MaxValue;

		[SyncState(StateLevel.Setup)]
		protected int Positions { get; set; }

		/// <summary>
		/// Outbound  and their direction
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public Dictionary<int, NodeDirection> ExitNodePositions { get; private set; } = new Dictionary<int, NodeDirection>();

		/// <summary>
		///Inbound edges and their positions
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public Dictionary<int, NodeDirection> EntranceNodePositions { get; private set; } = new Dictionary<int, NodeDirection>();

		[SyncState(StateLevel.Differential)]
		public Dictionary<int, VisitorPosition> VisitorPositions { get; private set; } = new Dictionary<int, VisitorPosition>();

		/// <summary>
		/// Routefinding dictionary, key by destination, returns the required exit connection
		/// </summary>
		[SyncState(StateLevel.Setup)]
		protected Dictionary<int, Connection> ExitRoutes { get; private set; }

		#region contructor

		public VisitableNodeBehaviour(IEntity entity, int positions) 
			: base(entity)
		{
			if (positions < MinPositions || positions > MaxPositions)
			{
				throw new ArgumentOutOfRangeException(nameof(positions), $"Value must be between {MinPositions} and {MaxPositions}, inclusive.");
			}

			Positions = positions;

			AddSubscription<EntityDestroyedMessage>(VisitorDestroyed);
		}


		#endregion

		#region visitor message handling

		private void VisitorDestroyed(EntityDestroyedMessage entityDestroyedMessage)
		{
			VisitorPositions.Remove(entityDestroyedMessage.Entity.Id);
		}

		#endregion

		public void SetRoutes(Dictionary<System, Connection[]> routes)
		{
			ExitRoutes = routes.ToDictionary(k => k.Key.Id, v => v.Value.First());
		}

		#region graph construction



		public void AddExitPosition(VertexDirection position, INode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException(nameof(node));
			}
			if (ExitNodePositions.ContainsKey(node.Id))
			{
				throw new Exception($"Exit connection is already registered at position {ExitNodePositions[node.Id]}");
			}
			ExitNodePositions.Add(node.Id, new NodeDirection(node, position));
		}

		public void AddEntrancePosition(VertexDirection position, INode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException(nameof(node));
			}
			if (EntranceNodePositions.ContainsKey(node.Id))
			{
				throw new Exception($"Entrance connection is already registered at position {EntranceNodePositions[node.Id]}");
			}
			EntranceNodePositions.Add(node.Id, new NodeDirection(node, position));
		}

		#endregion



		#region visitor movement methods

		public void AddVisitor(IVisitor visitor, INode source, int overflow)
		{
			// determine entrance position
			var sourceConnection = source as Connection;
			var sourceSystem = sourceConnection == null ? source : sourceConnection.Head;
			var direction = EntranceNodePositions.ContainsKey(sourceSystem.Id) ? EntranceNodePositions[sourceSystem.Id].Direction : VertexDirection.Top;

			var subscriptionsDisposable = new CompositeDisposable();
			// add to visitors
			var visitorPosition = new VisitorPosition(visitor, direction.ToPosition(Positions) + overflow, Entity.CurrentTick, subscriptionsDisposable);
			VisitorPositions.Add(visitor.Id, visitorPosition);

			// big TODO: we have a message loop here this isnt good

			// subscribe to messages from the visitor
			subscriptionsDisposable.Add(visitor.Subscribe(Entity));
			// subscribe the visitor to messages from this node
			subscriptionsDisposable.Add(Entity.Subscribe(visitor));
			
			var visitorEnteredNodeMessage = new VisitorEnteredNodeMessage(visitor, (INode) Entity);
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




		#region property getters

		public TVisitor GetVisitorOfType<TVisitor>()
			where TVisitor : class, IVisitor
		{
			return VisitorPositions.Values.SingleOrDefault(v => v.Visitor is TVisitor)?.Visitor as TVisitor;
		}

		public bool HasVisitorOfType<TVisitor>()
			where TVisitor : class, IVisitor
		{
			return VisitorPositions.Values.Any(v => v.Visitor is TVisitor);
		}

		public bool HasVisitor(IVisitor actor)
		{
			return VisitorPositions.ContainsKey(actor.Id);
		}

		#endregion

		protected virtual void VisitorLeaving(IVisitor visitor, INode exitNode, int overflow)
		{
			visitor.EntityDestroyed -= Visitor_EntityDestroyed;

			VisitorPositions[visitor.Id].VisitorSubscription.Dispose();
			VisitorPositions.Remove(visitor.Id);

			visitor.OnExitNode(Entity);

			// call add visitor on exit node and pass player
			exitNode.GetComponent<VisitableNodeBehaviour>().AddVisitor(visitor, Entity, overflow);
		}

		//public override void OnDeserialized()
		//{
		//	foreach (var visitorPosition in VisitorPositions.Values)
		//	{
		//		visitorPosition.Visitor.EntityDestroyed += Visitor_EntityDestroyed;
		//	}
		//}

		/// <summary>
		/// Get list of adjacent nodes (from outbound connections) 
		/// only used for virus spreading, so could be refactored
		/// </summary>
		/// <returns></returns>
		public List<INode> GetAdjacentNodes()
		{
			return ExitNodePositions.Values.Select(v => v.Node).ToList();
		}

		public VisitorPositionState GetState()
		{
			return VisitorPositions.Aggregate(new VisitorPositionState(), (vps, kvp) =>
			{
				vps.Add(kvp.Key, kvp.Value.Position);
				return vps;
			}); 
		}
	}
}
