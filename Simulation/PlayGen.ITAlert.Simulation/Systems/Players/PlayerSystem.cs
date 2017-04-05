using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Systems.Initialization;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Players
{
	public class PlayerSystem : IInitializingSystem
	{
		private readonly SimulationConfiguration _configuration;

		private readonly IEntityFactoryProvider _entityFactoryProvider;
		private readonly GraphSystem _graphSystem;
		private readonly MovementSystem _movementSystem;
		private readonly List<IPlayerSystemBehaviour> _playterSystemBehaviours;

		public PlayerSystem(SimulationConfiguration configuration, 
			IEntityFactoryProvider entityFactoryProvider,
			GraphSystem graphSystem, 
			MovementSystem movementSystem,
			[InjectOptional] List<IPlayerSystemBehaviour> playerSystemBehaviours)
		{
			_configuration = configuration;
			_entityFactoryProvider = entityFactoryProvider;
			_graphSystem = graphSystem;
			_movementSystem = movementSystem;
			_playterSystemBehaviours = playerSystemBehaviours;
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
					throw new SimulationException($"Could not create player for id '{playerConfig.EntityId}'");
				}
			}
		}

		public void PlayerDisconnected(int playerExternalId)
		{
			ExecuteBehaviourAction(psb => psb.OnPlayerDisconnected(playerExternalId));
		}

		public void PlayerJoined(int playerExternalId)
		{
			ExecuteBehaviourAction(psb => psb.OnPlayerJoined(playerExternalId));
		}

		private void ExecuteBehaviourAction(Action<IPlayerSystemBehaviour> action)
		{
			foreach (var behaviour in _playterSystemBehaviours)
			{
				action(behaviour);
			}
		}
	}
}
