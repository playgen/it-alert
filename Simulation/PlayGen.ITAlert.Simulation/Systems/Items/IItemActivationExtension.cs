using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Activation;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public interface IItemActivationExtension : ISystemExtension
	{
		ComponentMatcher Matcher { get; }

		void OnActivating(Entity item, Activation activation);

		void OnActive(Entity item, Activation activation);

		void OnDeactivating(Entity item, Activation activation);
	}
}
