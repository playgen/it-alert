using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Movement;
using PlayGen.ITAlert.Simulation.Configuration;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class SpawnNpcCommand : ICommand
	{
		public string Archetype { get; set; }

		public int SystemId { get; set; }

		public IdentifierType IdentifierType { get; set; }
	}

	public class SpawnNpcCommandHandler : CommandHandler<SpawnNpcCommand>
	{
		private readonly EntityFactoryProvider _entityFactoryProvider;

		private readonly IEntityRegistry _entityRegistry;

		private readonly Dictionary<int, int> _logicalIdMap;

		public SpawnNpcCommandHandler(EntityFactoryProvider entityFactoryProvider, 
			IEntityRegistry entityRegistry,
			SimulationConfiguration configuration)
		{
			_entityFactoryProvider = entityFactoryProvider;
			_entityRegistry = entityRegistry;
			_logicalIdMap = configuration.NodeConfiguration.ToDictionary(k => k.Id, v => v.EntityId);
		}

		protected override bool TryProcessCommand(SpawnNpcCommand command)
		{
			int systemEntityId;
			switch (command.IdentifierType)
			{
				case IdentifierType.Entity:
					systemEntityId = command.SystemId;
					break;
				case IdentifierType.Logical:
					if (_logicalIdMap.TryGetValue(command.SystemId, out systemEntityId) == false)
					{
						return false;
					}
					break;
				default:
					return false;
			} 
			Entity systemEntity;
			Visitors systemVisitors;
			if (string.IsNullOrEmpty(command.Archetype) == false
				&& _entityRegistry.TryGetEntityById(systemEntityId, out systemEntity)
				&& systemEntity.TryGetComponent(out systemVisitors))
			{
				Entity npc;
				CurrentLocation currentLocation;
				VisitorPosition visitorPosition;
				if (_entityFactoryProvider.TryCreateEntityFromArchetype(command.Archetype, out npc)
					&& npc.TryGetComponent(out currentLocation)
					&& npc.TryGetComponent(out visitorPosition))
				{
					systemVisitors.Values.Add(npc.Id);
					currentLocation.Value = systemEntity.Id;
					visitorPosition.SetPosition(0, 0);
					return true;
				}
			}
			return false;

		}
	}
}
