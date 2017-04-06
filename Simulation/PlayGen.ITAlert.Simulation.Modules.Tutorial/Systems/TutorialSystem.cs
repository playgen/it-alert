using System.Threading;
using Engine.Archetypes;
using PlayGen.ITAlert.Simulation.Components.EntityTypes;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Systems
{
	public class TutorialSystem : ITutorialSystem
	{
		public static Archetype TutorialState = new Archetype("TutorialState")
			.HasComponent(new ComponentBinding<NonUI>())
			.HasComponent(new ComponentBinding<TutorialState>());

		//private ComponentMatcherGroup<TutorialState> _tutorialStateMatcher;

		//private readonly IEntityFactoryProvider _entityFactoryProvider;

		//private TutorialState _tutorialState;

		//public bool Continue => _tutorialState?.Continue ?? false;

		private readonly AutoResetEvent _continue = new AutoResetEvent(false);

		public bool Continue => _continue.WaitOne(0);

		public TutorialSystem()// IMatcherProvider matcherProvider, IEntityFactoryProvider entityFactoryProvider)
		{
			//_tutorialStateMatcher = matcherProvider.CreateMatcherGroup<TutorialState>();
			//_entityFactoryProvider = entityFactoryProvider;
		}

		public void Initialize()
		{
			// TODO: an entity makes the system state serializable, but is that necessary?
			//Entity tutorialStateEntity;
			//if (_entityFactoryProvider.TryCreateEntityFromArchetype(nameof(TutorialState), out tutorialStateEntity)
			//	&& tutorialStateEntity.TryGetComponent(out _tutorialState) == false)
			//{
			//	throw new SimulationException($"Failed to initialize tutorial state entity");
			//}
		}

		public void SetContinue()
		{
			_continue.Set();
		}	
	}
}
