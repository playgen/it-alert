using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Events;
using Engine.Lifecycle.Events;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Scoring;
using PlayGen.ITAlert.Simulation.Scoring.Team.Events;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Scoring.Team
{
	public class TeamScoringSystem : ITickableSystem
	{
		private readonly List<ITeamScoringExtension> _scoringExtensions;

		public int  CumulativeScore { get; private set; }
		public List<decimal> SystemHealth { get; private set; } 

		private readonly EventSystem _eventSystem;
		private readonly ComponentMatcherGroup<Components.EntityTypes.Player, Score> _playerMatcherGroup;

		private readonly IDisposable _endGameSubscription;
		
		public TeamScoringSystem([InjectOptional]List<ITeamScoringExtension> scoringExtensions,
			IMatcherProvider matcherProvider,
			EventSystem eventSystem)
		{
			_scoringExtensions = scoringExtensions;
			_playerMatcherGroup = matcherProvider.CreateMatcherGroup<Components.EntityTypes.Player, Score>();

			SystemHealth = new List<decimal>();
			foreach (var extension in scoringExtensions)
			{
				extension.Score += ExtensionOnScore;
			}
			_eventSystem = eventSystem;
			_endGameSubscription = eventSystem.Subscribe<EndGameEvent>(OnEndGameEvent);
		}

		private void OnEndGameEvent(EndGameEvent @event)
		{
			var teamScoreEvent = new TeamScoreEvent()
			{
				Score = CumulativeScore,
			};
			_eventSystem.Publish(teamScoreEvent);
		}

		private void ExtensionOnScore(int i, decimal systemHealth = -1)
		{
			CumulativeScore += i;
			if (systemHealth != -1)
			{
				SystemHealth.Add(systemHealth);
			}
		}

		public List<Score> GetPlayerScores()
		{
			return _playerMatcherGroup.MatchingEntities.Select(s => s.Component2).ToList();
		}

		public void Tick(int currentTick)
		{
			foreach (var extension in _scoringExtensions.OfType<Engine.ITickable>())
			{
				extension.Tick(currentTick);
			}
		}

		public void Dispose()
		{
		}
	}
}
