using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems.Activation;
using Engine.Systems.Activation.Components;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ResetOwnerOnDeactivate : IActivationExtension
	{
		private readonly ComponentMatcherGroup<Activation, Owner> _ownerActivationMatcherGroup;

		public ResetOwnerOnDeactivate(IMatcherProvider matcherProvider)
		{
			_ownerActivationMatcherGroup = matcherProvider.CreateMatcherGroup<Activation, Owner>();
		}

		public void OnNotActive(int itemId, Activation activation)
		{

		}

		public void OnActivating(int itemId, Activation activation)
		{

		}

		public void OnActive(int itemId, Activation activation)
		{
			// do nothing
		}

		public void OnDeactivating(int itemId, Activation activation)
		{
			ComponentEntityTuple<Activation, Owner> itemTuple;
			if (_ownerActivationMatcherGroup.TryGetMatchingEntity(itemId, out itemTuple))
			{
				itemTuple.Component2.Value = null;
			}
		}
	}
}
