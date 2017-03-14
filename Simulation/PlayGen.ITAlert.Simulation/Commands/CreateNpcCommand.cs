﻿using System;
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
	public class CreateNpcCommand : ICommand
	{
		public string Archetype { get; set; }

		public int SystemId { get; set; }

		public IdentifierType IdentifierType { get; set; }
	}

	public class CreateNpcCommandHandler : CommandHandler<CreateNpcCommand>
	{
		private readonly IEntityFactoryProvider _entityFactoryProvider;

		private readonly IEntityRegistry _entityRegistry;

		private SimulationConfiguration _configuration;

		public CreateNpcCommandHandler(IEntityFactoryProvider entityFactoryProvider, 
			IEntityRegistry entityRegistry,
			SimulationConfiguration configuration)
		{
			_entityFactoryProvider = entityFactoryProvider;
			_entityRegistry = entityRegistry;
			_configuration = configuration;
		}

		protected override bool TryProcessCommand(CreateNpcCommand command)
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
				npc?.Dispose();
			}
			return false;

		}
	}
}
