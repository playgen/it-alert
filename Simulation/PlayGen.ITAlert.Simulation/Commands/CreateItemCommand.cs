using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
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

		public IdentifierType IdentifierType { get; set; }

		#region deduplication equality

		protected bool Equals(CreateItemCommand other)
		{
			return string.Equals(Archetype, other.Archetype) && SystemId == other.SystemId && IdentifierType == other.IdentifierType;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CreateItemCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Archetype != null ? Archetype.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ SystemId;
				hashCode = (hashCode * 397) ^ (int) IdentifierType;
				return hashCode;
			}
		}

		public bool Equals(ICommand other)
		{
			return Equals(other as CreateItemCommand);
		}

		#endregion
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
					var itemContainer = itemStorage.Items.FirstOrDefault(ic => ic != null && ic.Item.HasValue == false && ic.Enabled);
					if (itemContainer == null)
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
