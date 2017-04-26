using Engine.Components;
using Engine.Events;
using Engine.Systems.Scoring;
using PlayGen.ITAlert.Simulation.Components.Scoring;

namespace PlayGen.ITAlert.Scoring
{
	public abstract class PlayerScoringEventHandler<TEvent> : ScoringEventHandler<TEvent>
		where TEvent : class, IEvent
	{
		protected ComponentMatcherGroup<Simulation.Components.EntityTypes.Player, Score> PlayerScoreMatcherGroup { get; }

		protected PlayerScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider) 
			: base(eventSystem)
		{
			PlayerScoreMatcherGroup = matcherProvider.CreateMatcherGroup<Simulation.Components.EntityTypes.Player, Score>();
		}
	}
}

