using Engine.Components;
using Engine.Systems;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Systems
{
	public class ContinueSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Activation, ActivationContinue> _continueMatcherGroup;

		private readonly ITutorialSystem _tutorialSystem;

		public ContinueSystem(IMatcherProvider matcherProvider, ITutorialSystem tutorialSystem)
		{
			_continueMatcherGroup = matcherProvider.CreateMatcherGroup<Activation, ActivationContinue>();
			_tutorialSystem = tutorialSystem;
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _continueMatcherGroup.MatchingEntities)
			{
				if (match.Component1.ActivationState == match.Component2.ContinueOn)
				{
					_tutorialSystem.SetContinue();
				}
			}
		}
	}
}
