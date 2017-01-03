﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.bin;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Components.Properties;

namespace PlayGen.ITAlert.Simulation.Components.Systems
{
	public class ItemActivation : Engine.Systems.System
	{
		public ItemActivation(ComponentRegistry componentRegistry, EntityRegistry entityRegistry) 
			: base(componentRegistry, entityRegistry)
		{



		}

		public override void Tick(int currentTick)
		{
			var activations = ComponentRegistry.GetComponentEntitesImplmenting<Activation.Activation>();

			foreach (var activation in activations)
			{
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						// TODO: is activation pending?
						continue;
					case ActivationState.Activating:
						activation.SetState(ActivationState.Active);
						foreach (var activatable in activation.Entity.GetComponents<IActivatable>())
						{
							activatable.OnActivating();
						}
						break;
					case ActivationState.Deactivating:
						activation.SetState(ActivationState.NotActive);
						foreach (var activatable in activation.Entity.GetComponents<IActivatable>())
						{
							activatable.OnDeactivating();
						}
						break;
					case ActivationState.Active:
						foreach (var activatable in activation.Entity.GetComponents<IActivatable>())
						{
							activatable.OnActive();
						}
						break;
				}
			}


		}
	}
}
