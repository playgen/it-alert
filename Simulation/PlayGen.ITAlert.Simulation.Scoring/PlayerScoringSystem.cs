using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Systems.Scoring;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Components.Scoring;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Systems.Players;
using Zenject;

namespace PlayGen.ITAlert.Scoring
{
	public class PlayerScoringSystem : ScoringSystem
	{
		private readonly ComponentMatcherGroup<Player, Score> _playerMatcherGroup;

		private readonly PlayerSystem _playerSystem;

		public PlayerScoringSystem([InjectOptional] List<IScoringExtension> scoringExtensions,
			IMatcherProvider matcherProvider,
			PlayerSystem playerSystem)
			: base(scoringExtensions)
		{
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Player, Score>();
			_playerSystem = playerSystem;
		}

		public Score GetScoreForPlayer(int playerLogicalId)
		{
			if (_playerSystem.TryGetPlayerEntityId(playerLogicalId,
				out var playerEntityId))
			{
				return GetScoreForPlayerEntity(playerEntityId);
			}
			throw new SimulationException($"Could not get score for player {playerLogicalId}");
		}

		public Score GetScoreForPlayerEntity(int playerEntityId)
		{
			if (_playerMatcherGroup.TryGetMatchingEntity(playerEntityId,
				out var playerTuple))
			{
				return playerTuple.Component2;
			}
			throw new SimulationException($"Could not get score for entity {playerEntityId}");
		}

		public Score GetAggregateScore()
		{
			return _playerMatcherGroup.MatchingEntities.Aggregate(new Score(),
				(score, playerTuple) =>
				{
					score.Systematicity += playerTuple.Component2.Systematicity;
					score.ResourceManagement += playerTuple.Component2.ResourceManagement;
					return score;
				});
		}

		public override void Dispose()
		{
			_playerMatcherGroup?.Dispose();
			base.Dispose();
		}
	}
}
