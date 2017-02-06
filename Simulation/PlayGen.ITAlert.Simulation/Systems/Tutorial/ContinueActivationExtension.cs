using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Systems.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Tutorial
{
	public class ContinueActivationExtension : IItemActivationExtension
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
