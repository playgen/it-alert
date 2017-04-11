using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components;
using PlayGen.ITAlert.Simulation.Components.Player;
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

		private readonly Dictionary<int, int> _playerEntityMapping;

		private int _playerId;

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
			_playerEntityMapping = new Dictionary<int, int>();
		}

		#region Implementation of IInitializingSystem

		public void Initialize()
		{
			CreatePlayers(_configuration.PlayerConfiguration);
		}

		#endregion
		
		private void CreatePlayers(IEnumerable<PlayerConfig> playerConfigs)
		{
			foreach (var playerConfig in playerConfigs)
			{
				CreatePlayer(playerConfig);
			}
		}

		public bool TryGetPlayerEntityId(int playerId, out int playerEntityId)
		{
			return _playerEntityMapping.TryGetValue(playerId, out playerEntityId);
		}

		public Entity CreatePlayer(PlayerConfig playerConfig)
		{
			if (_entityFactoryProvider.TryCreateEntityFromArchetype(playerConfig.Archetype, out var player)
				&& player.TryGetComponent<PlayerBitMask>(out var playerBitMask)
				&& player.TryGetComponent<PlayerColour>(out var playerColour))
			{
				playerConfig.EntityId = player.Id;
				playerConfig.Id = _playerId;

				var startingLocationId = playerConfig.StartingLocation ?? 0;
				_movementSystem.AddVisitor(_graphSystem.Subsystems[startingLocationId], player);
				playerBitMask.Value = 1 << _playerId;
				playerColour.HexColour = playerConfig.Colour;

				_playerEntityMapping.Add(_playerId, player.Id);

				_playerId++;
				return player;
			}
			else
			{
				player?.Dispose();
				throw new SimulationException($"Could not create player for id '{playerConfig.EntityId}'");
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
