using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Systems.Movement;

namespace PlayGen.ITAlert.Simulation.Systems.Initialization
{
	public class PlayerSystem : IInitializingSystem
	{
		private readonly SimulationConfiguration _configuration;

		private readonly IEntityFactoryProvider _entityFactoryProvider;
		private readonly GraphSystem _graphSystem;
		private readonly MovementSystem _movementSystem;


		public PlayerSystem(SimulationConfiguration configuration, 
			IEntityFactoryProvider entityFactoryProvider,
			GraphSystem graphSystem, 
			MovementSystem movementSystem)
		{
			_configuration = configuration;
			_entityFactoryProvider = entityFactoryProvider;
			_graphSystem = graphSystem;
			_movementSystem = movementSystem;
		}

		#region Implementation of IInitializingSystem

		public void Initialize()
		{
			CreatePlayers(_graphSystem.Subsystems, _configuration.PlayerConfiguration);
		}

		#endregion
		
		private void CreatePlayers(Dictionary<int, Entity> subsystems, IEnumerable<PlayerConfig> playerConfigs)
		{
			if (playerConfigs.Any())
			{
				int playerId = 0;

				foreach (var playerConfig in playerConfigs)
				{
					if (_entityFactoryProvider.TryCreateEntityFromArchetype(playerConfig.ArchetypeName, out var player)
						&& player.TryGetComponent<PlayerBitMask>(out var playerBitMask))
					{
						playerConfig.EntityId = player.Id;
						var startingLocationId = playerConfig.StartingLocation ?? 0;
						_movementSystem.AddVisitor(subsystems[startingLocationId], player);
						playerBitMask.Value = 1 << playerId++;
						continue;
					}
					player?.Dispose();
					throw new SimulationException($"Could not craete player for id '{playerConfig.EntityId}'");
				}
			}
		}
	}
}
