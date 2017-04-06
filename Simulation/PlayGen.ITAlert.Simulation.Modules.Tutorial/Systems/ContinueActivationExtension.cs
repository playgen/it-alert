using Engine.Components;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Systems
{
	public class ContinueActivationExtension : IActivationExtension
	{
		private readonly ComponentMatcherGroup<ActivationContinue> _componentMatcherGroup;

		private readonly ITutorialSystem _tutorialSystem;

		public ContinueActivationExtension(IMatcherProvider matcherProvider, ITutorialSystem tutorialSystem)
		{
			_componentMatcherGroup = matcherProvider.CreateMatcherGroup<ActivationContinue>();
			_tutorialSystem = tutorialSystem;
		}

		private void TryContinueItem(int entityId, ActivationContinue.ActivationPhase activationPhase)
		{
			ComponentEntityTuple<ActivationContinue> activationTuple;
			if (_componentMatcherGroup.TryGetMatchingEntity(entityId, out activationTuple)
				&& activationTuple.Component1.ContinueOn == activationPhase)
			{
				_tutorialSystem.SetContinue();
			}
		}

		public void OnNotActive(int itemId, Activation activation)
		{

		}

		public void OnActivating(int itemId, Activation activation)
		{
			TryContinueItem(itemId, ActivationContinue.ActivationPhase.Activating);
		}

		public void OnActive(int itemId, Activation activation)
		{
			// TODO: this could be dangerous as it will continue to set the waithandle while the item remains active
			// TryContinueItem(itemId, ActivationContinue.ActivationPhase.Active);
		}

		public void OnDeactivating(int itemId, Activation activation)
		{
			TryContinueItem(itemId, ActivationContinue.ActivationPhase.Deactivating);
		}
	}
}
