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
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class CreateItemCommand : ICommand
	{
		public string Archetype { get; set; }

		public int SystemId { get; set; }

		public Type ContainerType { get; set; }
	}

	public class CreateItemCommandHandler : CommandHandler<CreateItemCommand>
	{
		private readonly IEntityFactoryProvider _entityFactoryProvider;

		private readonly ComponentMatcherGroup<Subsystem, ItemStorage> _subsystemMatcherGroup;

		public CreateItemCommandHandler(IEntityFactoryProvider entityFactoryProvider, 
			IMatcherProvider matcherProvider)
		{
			_entityFactoryProvider = entityFactoryProvider;
			_subsystemMatcherGroup = matcherProvider.CreateMatcherGroup<Subsystem, ItemStorage>();
		}

		protected override bool TryProcessCommand(CreateItemCommand command)
		{
			if (string.IsNullOrEmpty(command.Archetype) == false
				&& _subsystemMatcherGroup.TryGetMatchingEntity(command.SystemId, out var systemTuple)
				&& _entityFactoryProvider.TryCreateItem(command.Archetype, command.SystemId, null, out var itemTuple))
			{
				if (command.ContainerType != null
					&& systemTuple.Component2.TryGetItemContainer(command.ContainerType, out var itemContainer)
					&& itemContainer.CanCapture(itemTuple.Entity.Id))
				{
					itemContainer.Item = itemTuple.Entity.Id;
					return true;
				}
				if (systemTuple.Component2.TryGetEmptyContainer(out var emptyItemContainer, out var containerIndex))
				{
					emptyItemContainer.Item = itemTuple.Entity.Id;
					return true;
				}
				itemTuple.Entity.Dispose();
				return false;
			}
			return false;
		}
	}
}
