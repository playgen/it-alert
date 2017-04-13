using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Players
{
	public class DropInventoryOnDisconnect : IPlayerSystemBehaviour
	{
		private readonly ComponentMatcherGroup<Player, ItemStorage, CurrentLocation> _playerMatcherGroup;
		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;
		private readonly ComponentMatcherGroup<Connection, GraphNode> _connectionMatcherGroup;
		private readonly ComponentMatcherGroup<Item, Owner, CurrentLocation> _itemMatcherGroup;


		private readonly SimulationConfiguration _configuration;

		// TODO: temporary workaround for circular dependency
		// CommandQueue -> CommandSystem -> CreatePlayerCommandHandler -> Playersystem -> this

		public DropInventoryOnDisconnect(SimulationConfiguration configuration, 
			IMatcherProvider matcherProvider)
		{
			_configuration = configuration;
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, ItemStorage, CurrentLocation>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_connectionMatcherGroup = matcherProvider.CreateMatcherGroup<Connection, GraphNode>();
			_itemMatcherGroup = matcherProvider.CreateMatcherGroup<Item, Owner, CurrentLocation>();
		}

		#region Implementation of IPlayerSystemBehaviour

		public void OnPlayerJoined(int playerExternalId)
		{
		}

		public void OnPlayerDisconnected(int playerExternalId)
		{
			var player = _configuration.PlayerConfiguration.SingleOrDefault(p => p.ExternalId == playerExternalId);
			if (player != null)
			{
				if (_playerMatcherGroup.TryGetMatchingEntity(player.EntityId, out var playerTuple)
					&& playerTuple.Component3.Value.HasValue
					&& playerTuple.Component2.TryGetItemContainer<InventoryItemContainer>(out var inventoryItemContainer)
					&& inventoryItemContainer.Item.HasValue
					)
				{
					if (_subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var subsystemTuple))
					{
						if (TryHandleDisconectOnSubsystem(subsystemTuple, playerTuple, inventoryItemContainer) == false)
						{
							
						}
					}
					else if (_connectionMatcherGroup.TryGetMatchingEntity(playerTuple.Component3.Value.Value, out var connectionTuple))
					{
						
					}

				}


			}

			// do nothing
		}

		private bool TryHandleDisconectOnSubsystem(ComponentEntityTuple<Subsystem, ItemStorage> subsystemTuple,
			ComponentEntityTuple<Player, ItemStorage, CurrentLocation> playerTuple, 
			InventoryItemContainer inventoryItemContainer)
		{
			if (subsystemTuple.Component2.TryGetEmptyContainer(out var emptyContainer, out var containerIndex))
			{
				#region copied from DropItemCommandHandler
				// TODO: this should be replaced with an intent based approach, the command handler should abstract through the intent as well

				if (_itemMatcherGroup.TryGetMatchingEntity(inventoryItemContainer.Item.Value, out var itemTuple)
					&& playerTuple.Component3.Value.HasValue
					&& itemTuple.Component2.Value == playerTuple.Entity.Id)
				{
					var inventory = playerTuple.Component2.Items[0] as InventoryItemContainer;
					var target = subsystemTuple.Component2.Items[containerIndex];
					if (inventory != null
						&& inventory.Item == itemTuple.Entity.Id
						&& target != null
						&& target.CanCapture(itemTuple.Entity.Id))
					{
						target.Item = itemTuple.Entity.Id;
						itemTuple.Component3.Value = subsystemTuple.Entity.Id;
						inventory.Item = null;
						itemTuple.Component2.Value = null;
						return true;
					}
				}
				#endregion
			}
			return false;
		}

		#endregion
	}
}
