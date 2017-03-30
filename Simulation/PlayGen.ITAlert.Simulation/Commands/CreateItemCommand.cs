using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
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

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		private readonly SimulationConfiguration _configuration;

		public CreateItemCommandHandler(IEntityFactoryProvider entityFactoryProvider, 
			IMatcherProvider matcherProvider,
			SimulationConfiguration configuration)
		{
			_entityFactoryProvider = entityFactoryProvider;
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
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
			if (string.IsNullOrEmpty(command.Archetype) == false
				&& _subsystemMatcherGroup.TryGetMatchingEntity(systemEntityId, out var systemTuple)
				&& _entityFactoryProvider.TryCreateItem(command.Archetype, systemEntityId, null, out var itemTuple))
			{
				if (systemTuple.Component2.TryGetEmptyContainer(out var itemContainer, out var containerIndex) == false)
				{
					itemTuple.Entity.Dispose();
					return false;
				}
				itemContainer.Item = itemTuple.Entity.Id;
				return true;
			}
			return false;

		}
	}
}
