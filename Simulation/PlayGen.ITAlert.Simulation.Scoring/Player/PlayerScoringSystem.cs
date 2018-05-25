using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Events;
using Engine.Lifecycle.Events;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Scoring;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Scoring.Player.Events;
using PlayGen.ITAlert.Simulation.Systems.Players;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Scoring.Player
{
	public class PlayerScoringSystem : ITickableSystem
	{
		private readonly List<IPlayerScoringExtension> _scoringExtensions;

		private readonly ComponentMatcherGroup<Components.EntityTypes.Player, Score> _playerMatcherGroup;

		private readonly PlayerSystem _playerSystem;
		private readonly EventSystem _eventSystem;

		private readonly IDisposable _endGameSubscription;

		public PlayerScoringSystem([InjectOptional] List<IPlayerScoringExtension> scoringExtensions,
			IMatcherProvider matcherProvider,
			PlayerSystem playerSystem,
			EventSystem eventSystem)
		{
			_scoringExtensions = scoringExtensions;
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Components.EntityTypes.Player, Score>();
			_playerSystem = playerSystem;
			_eventSystem = eventSystem;

			_endGameSubscription = eventSystem.Subscribe<EndGameEvent>(OnEndGameEvent);
		}

		private void OnEndGameEvent(EndGameEvent @event)
		{
			foreach (var playerTuple in _playerMatcherGroup.MatchingEntities)
			{
				var playerScoreEvent = new PlayerScoreEvent()
				{
					PlayerEntityId = playerTuple.Entity.Id,
					ResourceManagement = playerTuple.Component2.ResourceManagement,
					Systematicity = playerTuple.Component2.Systematicity,
				};
				_eventSystem.Publish(playerScoreEvent);
			}
		}

		public Score GetScoreForPlayer(int playerLogicalId)
		{
			if (_playerSystem.TryGetPlayerEntityId(playerLogicalId,
				out var playerEntityId))
			{
				return GetScoreForPlayerEntity(playerEntityId);
			}
			return new Score();
		}

		public Score GetScoreForPlayerEntity(int playerEntityId)
		{
			if (_playerMatcherGroup.TryGetMatchingEntity(playerEntityId,
				out var playerTuple))
			{
				return playerTuple.Component2;
			}
			return new Score();
		}

		public Score GetAggregateScore()
		{
			return _playerMatcherGroup.MatchingEntities.Aggregate(new Score(),
				(score, playerTuple) =>
				{
					score.Systematicity += playerTuple.Component2.Systematicity;
					score.ResourceManagement += playerTuple.Component2.ResourceManagement;
					score.PublicScore += playerTuple.Component2.PublicScore;
					return score;
				});
		}

		public void Dispose()
		{
			_playerMatcherGroup?.Dispose();
			_endGameSubscription?.Dispose();
		}

		public void Tick(int currentTick)
		{
			foreach (var tickableExtension in _scoringExtensions.OfType<ITickable>())
			{
				tickableExtension.Tick();
			}
		}
	}
}
