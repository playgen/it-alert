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
		void OnNotActive(int itemId, Activation activation);

		void OnActivating(int itemId, Activation activation);

		void OnActive(int itemId, Activation activation);

		void OnDeactivating(int itemId, Activation activation);
	}
}
