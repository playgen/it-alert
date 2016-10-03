using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Common;
using PlayGen.ITAlert.Common.Serialization;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Contracts.Extensions;
using PlayGen.ITAlert.Simulation.Intents;
using PlayGen.ITAlert.Simulation.Interfaces;
using PlayGen.ITAlert.Simulation.Utilities;
using PlayGen.ITAlert.Simulation.Visitors.Actors;
using PlayGen.ITAlert.Simulation.Visitors.Items;
using PlayGen.ITAlert.Simulation.World.Enhancements;

namespace PlayGen.ITAlert.Simulation.World
{
	public class Subsystem : Node<SubsystemState>
	{
		/// <summary>
		/// Routefinding dictionary, key by destination, returns the required exit connection
		/// </summary>
		[SyncState(StateLevel.Setup)]
		protected Dictionary<int, Connection> ExitRoutes { get; private set; }

		#region layout

		/// <summary>
		/// logical coordinate x axis
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public int X { get; private set; }
		/// <summary>
		/// logical coordinate y axis
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public int Y { get; private set; }

		[SyncState(StateLevel.Setup)]
		public int LogicalId { get; set; }

		#endregion

		#region items

		/// <summary>
		/// Maximum number of items on subsystem
		/// </summary>
		private const int ItemLimit = 4;

		/// <summary>
		/// Items currently on subsystem
		/// This is an array as the position is important for rendering in consistent coreners
		/// </summary>
		[SyncState(StateLevel.Full)]
		public IItem[] Items { get; private set; } = new IItem[ItemLimit];

		public bool HasActiveItem => Items.Any(i => i != null && i.IsActive);

		#endregion

		#region Health

		/// <summary>
		/// Current health of the subsystem
		/// </summary>
		[SyncState(StateLevel.Full)]
		public int Health { get; private set; }


		/// <summary>
		/// Current health of the subsystem
		/// </summary>
		[SyncState(StateLevel.Full)]
		public int Shield { get; private set; }

		#endregion

		/// <summary>
		/// The name of this subsystem
		/// </summary>
		[SyncState(StateLevel.Setup)]
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		private SubsystemEnhancement _enhancement;

		#region constructors

		/// <summary>
		/// /
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="logicalId"></param>
		/// <param name="enhancement"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Subsystem(ISimulation simulation, int logicalId, SubsystemEnhancement enhancement, int x, int y) 
			: this(simulation, logicalId, enhancement, SimulationConstants.Positions, x, y)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="simulation"></param>
		/// <param name="logicalId"></param>
		/// <param name="enhancement"></param>
		/// <param name="positions"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		protected Subsystem(ISimulation simulation, int logicalId, SubsystemEnhancement enhancement, int positions, int x, int y)
			: base(simulation, EntityType.Subsystem, positions)
		{
			LogicalId = logicalId;
			_enhancement = enhancement;
			//TODO: allow setting initial health

			Health = SimulationConstants.MaxHealth;
			Shield = SimulationConstants.MaxShield;

			X = x;
			Y = y;
		}

		/// <summary>
		/// Serialization constructor
		/// </summary>
		[Obsolete("This is not obsolete; it should never be called explicitly, it only exists for the serializer.", true)]
		protected Subsystem()
		{

		}

		#endregion

		#region Initialization

		public void SetRoutes(Dictionary<Subsystem, Connection[]> routes)
		{
			ExitRoutes = routes.ToDictionary(k => k.Key.Id, v => v.Value.First());
		}

		#endregion

		#region Graph traversal

		/// <summary>
		/// Get list of adjacent nodes (from outbound connections) 
		/// only used for virus spreading, so could be refactored
		/// </summary>
		/// <returns></returns>
		public List<INode> GetAdjacentNodes()
		{
			return ExitNodePositions.Values.Select(v => v.Node).ToList();
		}

		#endregion

		#region Evaluation

		//TODO: create extensible evaluators so that these are not hardcoded proeprties of the system

		//TODO: handle other type of infection
		/// <summary>
		/// Is this node infected by something
		/// </summary>
		public bool IsInfected => HasVisitorOfType<IInfection>();

		/// <summary>
		/// Has this system been killed
		/// </summary>
		public bool IsDead => Health == 0;

		#endregion

		#region visitors

		/// <summary>
		/// 
		/// </summary>
		/// <param name="visitor">The visitor entering the node</param>
		/// <param name="source">The node the visitor is entering from</param>
		/// <param name="offset">Overflow movement points</param>
		public override void AddVisitor(IVisitor visitor, INode source, int offset)
		{
			// TODO: this shouldnt be necessary as a virus shouldnt try to infect an already infected system
			var virus = visitor as Virus;
			if (virus != null && HasVisitorOfType<IInfection>() == false)
			{
				// don't add multiple viruses.
				VisitorPositions.Add(virus.Id, new VisitorPosition(virus, 0, 0));
			}
			else
			{
				virus?.Dispose();
			}

			var player = visitor as Player;
			if (player != null)
			{
				var sourceConnection = source as Connection;
				var sourceSubsystem = sourceConnection == null ? source : sourceConnection.Head;
				var position = source != null && EntranceNodePositions.ContainsKey(sourceSubsystem.Id) ? EntranceNodePositions[sourceSubsystem.Id].Direction : VertexDirection.Top;
				AddVisitor(visitor, position, offset);
				
				//OnPlayerEnterDestination(player);
			}

			// always call the base implementation
			base.AddVisitor(visitor, source, offset);
		}

		private void AddVisitor(IVisitor visitor, VertexDirection direction, int offset)
		{
			var visitorPosition = new VisitorPosition(visitor, direction.ToPosition(Positions) + offset, CurrentTick);

			VisitorPositions.Add(visitor.Id, visitorPosition);
		}

		//private void OnPlayerEnterDestination(Player player)
		//{
		//	Intent intent;
		//	if (player.TryGetIntent(out intent))
		//	{
		//		var moveIntent = intent as MoveIntent;
		//		if (moveIntent != null && moveIntent.Destination == this)
		//		{
		//			player.
		//			OnPlayerEnterWithItem(player);
		//		}
		//	}
		//}

		//private void OnPlayerEnterWithItem(Player player)
		//{
		//	if (player.Item != null && TryAddItem(player.Item))
		//	{
		//		player.DisownItem();
		//	}
		//}

		#region external modulators

		//TODO: there is probably a better pattern than this
		/// <summary>
		/// Apply a delta to the current health of this susbsystem
		/// currently used by repair items and viruses
		/// </summary>
		/// <param name="value"></param>
		public void ModulateHealth(int value)
		{
			if (value > 0)
			{
				if (Health < SimulationConstants.MaxHealth)
				{
					var overflow = value - (SimulationConstants.MaxHealth - Health);
					Health = Math.Min(SimulationConstants.MaxHealth, Health + value);
					if (overflow > 0)
					{
						Shield = Math.Min(SimulationConstants.MaxShield, Shield + overflow);
					}
				}
				else
				{
					Shield = Math.Min(SimulationConstants.MaxShield, Shield + value);
				}
			}
			else
			{
				if (Shield > 0)
				{
					var overflow = Shield + value;
					Shield = Math.Max(0, Shield + value);
					if (overflow < 0)
					{
						Health = Math.Max(0, Health + overflow);
					}
				}
				else
				{
					Health = Math.Max(0, Health + value);
				}
			}
		}

		#endregion

		#endregion

		#region items 

		/// <summary>
		/// Request an item of specified type if it is available on this subsystem, item will be returned in out parameter if available
		/// </summary>
		/// <param name="itemType">Type of item requested</param>
		/// <param name="requestor"></param>
		/// <param name="item">Item reference if available</param>
		/// <returns>Boolean indiciting whether item was obtained</returns>
		public bool TryGetItem(ItemType itemType, IActor requestor, out IItem item)
		{
			//TODO: probably lock here if we get multiple callers
			for (var i = 0; i < ItemLimit; i++)
			{
				if (TryGetItem(itemType, requestor, i, out item))
				{
					return true;
				}
			}
			item = null;
			return false;
		}

		/// <summary>
		/// Request an item of specified type if it is available on this subsystem, item will be returned in out parameter if available
		/// </summary>
		/// <param name="itemType">Type of item requested</param>
		/// <param name="itemLocation"></param>
		/// <param name="item">Item reference if available</param>
		/// <param name="requestor"></param>
		/// <returns>Boolean indiciting whether item was obtained</returns>
		public bool TryGetItem(ItemType itemType, IActor requestor, int itemLocation, out IItem item)
		{
			var tempItem = Items[itemLocation];
			if (tempItem != null 
				&& tempItem.ItemType == itemType 
				&& (tempItem.HasOwner == false || tempItem.Owner.Equals(requestor)))
			{
				item = tempItem;

				return true;
			}
			item = null;
			return false;
		}

		public void GetItem(IItem item)
		{
			for (var i = 0; i < ItemLimit; i++)
			{
				var storedItem = Items[i];
				if (item.Equals(storedItem))
				{
					Items[i] = null;
					storedItem.OnExitNode(this);
				}
			}
		}
		
		/// <summary>
		/// Determine whether the subsystem can accommodate any more items
		/// </summary>
		/// <returns></returns>
		public bool CanAddItem()
		{
			return Items.Any(i => i == null);
		}

		public bool HasItem(IItem item)
		{
			return Items.Contains(item);
		}

		public bool TryAddItem(IItem item)
		{
			if (item != null)
			{
				for (var i = 0; i < ItemLimit; i++)
				{
					var tempItem = Items[i];
					if (tempItem == null)
					{
						Items[i] = item;
						item.OnEnterNode(this);
						return true;
					}
				}
			}
			return false;
		}


		#endregion

		#region state snapshot

		public override SubsystemState GenerateState()
		{
			// return values that only this class knows about, anything else will be in the other entity's state
			var playerPositions = VisitorPositions
				.Where(v => v.Value.Visitor is Player)
				.ToDictionary(k => k.Key, v => v.Value.Position);

			var infection = VisitorPositions.Values.SingleOrDefault(v => v.Visitor is IInfection)?.Visitor as IInfection;

			var state = new SubsystemState(Id)
			{
				LogicalId = LogicalId,
				// static
				X = X,
				Y = Y,
				Name = Name,
				// ui state
				Health = (float)Health /SimulationConstants.MaxHealth,
				Shield = (float)Shield /SimulationConstants.MaxShield,
				Disabled = IsDead,
				//Infection = infection?.Visible ?? false ? infection.Id : (int?)null,
				Infection = infection?.Id,
				VisitorPositions = playerPositions,
				ItemPositions = Items.Select(i => i?.Id).ToArray(),
				HasActiveItem = HasActiveItem,	
			};
			return state;
		}

		public override void OnDeserialized()
		{
			base.OnDeserialized();

			foreach (var item in Items.Where(i => i != null))
			{
				item.EntityDestroyed += Item_EntityDestroyed;
			}
		}

		private void Item_EntityDestroyed(object sender, EventArgs e)
		{
			int index;
			if (Items.TryGetItemIndex(sender as IItem, out index))
			{
				Items[index] = null;
			}
		}

		#endregion

		#region Tick / Movement

		protected override void OnTick()
		{
			foreach (var visitorPosition in VisitorPositions.ToArray())
			{
				var visitor = visitorPosition.Value.Visitor;

				var player = visitor as Player;
				if (player != null)
				{
					OnTickPlayer(player, visitorPosition.Value);
					continue;
				}

				var virus = visitor as Virus;
				if (virus != null)
				{
					OnTickVirus(virus);
					continue;
				}

				var item = visitor as IItem;
				if (item != null)
				{
					OnTickItem(item);
					continue;
				}
			}
		}

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
			visitorPosition.UpdatePosition((visitorPosition.Position + player.Speed) % Positions, CurrentTick);
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
			var destination = intent.Destination as Subsystem;
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
