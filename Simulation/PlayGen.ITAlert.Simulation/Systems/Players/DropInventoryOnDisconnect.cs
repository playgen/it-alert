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
using PlayGen.ITAlert.Simulation.Systems.Extensions;

namespace PlayGen.ITAlert.Simulation.Systems.Players
{
	public class DropInventoryOnDisconnect : IPlayerSystemBehaviour
	{
		private readonly ComponentMatcherGroup<Player, CurrentLocation, ItemStorage> _playerMatcherGroup;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly ComponentMatcherGroup<Connection, GraphNode> _connectionMatcherGroup;

		private readonly SimulationConfiguration _configuration;

		private readonly CommandQueue _commandQueue;

		public DropInventoryOnDisconnect(SimulationConfiguration configuration, 
			IMatcherProvider matcherProvider, 
			CommandQueue commandQueue)
		{
			_configuration = configuration;
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, CurrentLocation, ItemStorage>();
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
			_connectionMatcherGroup = matcherProvider.CreateMatcherGroup<Connection, GraphNode>();
			_commandQueue = commandQueue;
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
					&& playerTuple.Component2.Value.HasValue
					&& playerTuple.Component3.TryGetItemContainer<InventoryItemContainer>(out var inventoryItemContainer)
					&& inventoryItemContainer.Item.HasValue)
				{
					if (_subsystemMatcherGroup.TryGetMatchingEntity(playerTuple.Component2.Value.Value, out var subsystemTuple))
					{
						if (subsystemTuple.Component2.TryGetEmptyContainer(out var emptyContainer, out var containerIndex))
						{
							// TODO: this should be replaced with an intent based approach, the command handler should abstract through the intent as well
							_commandQueue.EnqueueCommand(new DropItemCommand()
							{
								ContainerId = containerIndex,
								ItemId = inventoryItemContainer.Item.Value,
								PlayerId = player.EntityId,
							});
						}
						else
						{
							// TODO: there are no available storage locations on the current system
						}
					}
					else if (_connectionMatcherGroup.TryGetMatchingEntity(playerTuple.Component2.Value.Value, out var connectionTuple))
					{
						
					}

				}


			}

			// do nothing
		}

		#endregion
	}
}
