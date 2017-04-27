using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Components.Scoring;

namespace PlayGen.ITAlert.Simulation.Scoring.Player
{
	public abstract class PlayerScoringEventHandler<TEvent> : EventHandler<TEvent>, IPlayerScoringExtension
		where TEvent : class, IEvent
	{
		protected ComponentMatcherGroup<ITAlert.Simulation.Components.EntityTypes.Player, Score> PlayerScoreMatcherGroup { get; }

		protected PlayerScoringEventHandler(EventSystem eventSystem, IMatcherProvider matcherProvider) 
			: base(eventSystem)
		{
			PlayerScoreMatcherGroup = matcherProvider.CreateMatcherGroup<ITAlert.Simulation.Components.EntityTypes.Player, Score>();
		}
	}
}

