using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.Engine;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.World
{
	public abstract class Node<TState> : ITAlertEntity<TState>, INode
		where TState : ITAlertEntityState
	{
		#region events

		public event EventHandler<VistorEventArgs> VisitorEntered;

		#endregion

		private const int MinPositions = 1;
		private const int MaxPositions = int.MaxValue;

		[SyncState(StateLevel.Setup)]
		protected int Positions { get; set; }

		/// <summary>
		/// Outbound edges and their direction
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public Dictionary<int, NodeDirection> ExitNodePositions { get; private set; } = new Dictionary<int, NodeDirection>();

		/// <summary>
		///Inbound edges and their positions
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public Dictionary<int, NodeDirection> EntranceNodePositions { get; private set; } = new Dictionary<int, NodeDirection>();

		[SyncState(StateLevel.Full)]
		public Dictionary<int, VisitorPosition> VisitorPositions { get; private set; } = new Dictionary<int, VisitorPosition>();


		#region constructors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="entityType"></param>
		/// <param name="positions"></param>
		protected Node(ISimulation simulation, EntityType entityType, int positions)
			: base(simulation, entityType)
		{
			if (positions < MinPositions || positions > MaxPositions)
			{
				throw new ArgumentOutOfRangeException(nameof(positions), $"Value must be between {MinPositions} and {MaxPositions}, inclusive.");
			}

			Positions = positions;
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Node()
		{

		}

		#endregion

		#region initialization

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

		public virtual void AddVisitor(IVisitor visitor, INode source, int overflow)
		{
			visitor.EntityDestroyed += Visitor_EntityDestroyed;
			visitor.OnExitNode(source);
			visitor.OnEnterNode(this);
			OnVisitorEntered(new VistorEventArgs(visitor, source));
		}

		/// <summary>
		/// handle the entity destroyed event for our visitors as they need to be removed from the collection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Visitor_EntityDestroyed(object sender, EventArgs e)
		{
			var destroyedEntity = sender as IEntity;
			if (destroyedEntity != null)
			{
				VisitorPositions.Remove(destroyedEntity.Id);
			}
		}

		protected virtual void OnVisitorEntered(VistorEventArgs e)
		{
			VisitorEntered?.Invoke(this, e);
		}

		protected virtual void VisitorLeaving(IVisitor visitor, INode exitNode, int overflow)
		{
			visitor.EntityDestroyed -= Visitor_EntityDestroyed;
			VisitorPositions.Remove(visitor.Id);
			// call add visitor on exit node and pass player
			exitNode.AddVisitor(visitor, this, overflow);
		}

		public override void OnDeserialized()
		{
			foreach (var visitorPosition in VisitorPositions.Values)
			{
				visitorPosition.Visitor.EntityDestroyed += Visitor_EntityDestroyed;
			}
		}
	}
}
