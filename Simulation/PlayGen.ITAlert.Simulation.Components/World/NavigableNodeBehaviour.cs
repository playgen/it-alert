using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.Engine.Entities;
using PlayGen.Engine.Serialization;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Entities.Visitors;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Components.World
{
	[ComponentUsage(typeof(INode))]
	public class NavigableNodeBehaviour : ComponentBase
	{
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

		[SyncState(StateLevel.Differential)]
		public Dictionary<int, VisitorPosition> VisitorPositions { get; private set; } = new Dictionary<int, VisitorPosition>();

		public NavigableNodeBehaviour(IComponentContainer container, int positions) : base(container)
		{
			Positions = positions;
		}

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
			//VisitorEntered?.Invoke(this, e);
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
