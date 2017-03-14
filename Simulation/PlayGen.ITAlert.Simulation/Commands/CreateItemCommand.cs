using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Systems.Extensions;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class CreateItemCommand : ICommand
	{
		public string Archetype { get; set; }

		public int SystemId { get; set; }

		public IdentifierType IdentifierType { get; set; }
	}

	public class CreateItemCommandHandler : CommandHandler<CreateItemCommand>
	{
		private readonly IEntityFactoryProvider _entityFactoryProvider;

		private readonly IEntityRegistry _entityRegistry;

		private SimulationConfiguration _configuration;

		public CreateItemCommandHandler(IEntityFactoryProvider entityFactoryProvider, 
			IEntityRegistry entityRegistry,
			SimulationConfiguration configuration)
		{
			_entityFactoryProvider = entityFactoryProvider;
			_entityRegistry = entityRegistry;
			_configuration = configuration;
		}

		protected override bool TryProcessCommand(CreateItemCommand command)
		{
			int systemEntityId;
			switch (command.IdentifierType)
			{
				case IdentifierType.Entity:
					systemEntityId = command.SystemId;
					break;
				case IdentifierType.Logical:
					var nodeConfig = _configuration.NodeConfiguration.SingleOrDefault(nc => nc.Id == command.SystemId);
					if (nodeConfig == null)
					{
						return false;
					}
					systemEntityId = nodeConfig.EntityId;
					break;
				default:
					return false;
			} 
			Entity systemEntity;
			ItemStorage itemStorage;
			if (string.IsNullOrEmpty(command.Archetype) == false
				&& _entityRegistry.TryGetEntityById(systemEntityId, out systemEntity)
				&& systemEntity.TryGetComponent(out itemStorage))
			{
				ComponentEntityTuple<CurrentLocation, Owner> itemTuple;
				if (_entityFactoryProvider.TryCreateItem(command.Archetype, systemEntityId, null, out itemTuple))
				{
					ItemContainer itemContainer;
					if (itemStorage.TryGetEmptyContainer(out itemContainer) == false)
					{
						itemTuple.Entity.Dispose();
						return false;
					}
					itemContainer.Item = itemTuple.Entity.Id;
					return true;
				}
			}
			return false;

		}
	}
}
