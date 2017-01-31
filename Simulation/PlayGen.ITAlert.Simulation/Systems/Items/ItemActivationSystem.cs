using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Engine.Util;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Items;
using PlayGen.ITAlert.Simulation.Systems.Malware;
using PlayGen.ITAlert.Simulation.Systems.Planning;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ItemActivationSystem : Engine.Systems.System, ITickableSystem
	{
		private IntentSystem _intentSystem;

		private readonly List<IItemActivationExtension> _itemActivationExtensions;
		
		private readonly ComponentMatcherGroup<Activation> _activationMatcher;

		public ItemActivationSystem(IComponentRegistry componentRegistry, 
			IEntityRegistry entityRegistry, 
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<IItemActivationExtension> itemActivationExtensions,
			IntentSystem intentSystem) 

			: base(componentRegistry, entityRegistry)
		{
			_itemActivationExtensions = itemActivationExtensions;
			_intentSystem = intentSystem;
			
			_activationMatcher = componentRegistry.CreateMatcherGroup<Activation>();
			componentRegistry.RegisterMatcher(_activationMatcher);
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _activationMatcher.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						// TODO: is activation pending?
						continue;
					case ActivationState.Activating:
						activation.SetState(ActivationState.Active);
						// TODO: confirm that this makes sense - process system extensions, then any components that are activatable
						ExecuteActivationExtensionActions(match.Entity, iax => iax.OnActivating(match.Entity, activation));

						//ExecuteActivatableAction(entity, a => a.OnActivating(entity));
						break;
					case ActivationState.Deactivating:
						activation.SetState(ActivationState.NotActive);
						ExecuteActivationExtensionActions(match.Entity, iax => iax.OnDeactivating(match.Entity, activation));

						//ExecuteActivatableAction(entity, a => a.OnDeactivating(entity));
						break;
					case ActivationState.Active:
						ExecuteActivationExtensionActions(match.Entity, iax => iax.OnActive(match.Entity, activation));

						//ExecuteActivatableAction(entity, a => a.OnActive(entity));
						break;
				}
			}
		}

		private void ExecuteActivatableAction(Entity entity, Action<IActivatable> action)
		{
			foreach (var activatable in entity.GetComponents<IActivatable>())
			{
				action(activatable);
			}
		}
		
		private void ExecuteActivationExtensionActions(Entity entity, Action<IItemActivationExtension> action)
		{
			foreach (var activationExtension in _itemActivationExtensions)
			{
				action(activationExtension);
			}
		}

		#region command/activation logic

		// TODO: decide if this is where these really belong

		public static bool CanActorActivateItem(Entity actor, Entity item)
		{
			Owner owner;
			return item.TryGetComponent(out owner)
					&& owner.Value == actor.Id
					&& CanActivateItem(item);
		}

		public static bool CanActivateItem(Entity item)
		{
			var activation = item.GetComponent<Activation>();
			return activation.ActivationState == ActivationState.NotActive;
		}

		public static bool TryActivateItem(Entity actor, Entity item)
		{
			// TODO: test ownership (probably)
			if (CanActorActivateItem(actor, item))
			{
				var activation = item.GetComponent<Activation>();
				activation.SetState(ActivationState.Activating);
				return true;
			}
			return false;
		}

		#endregion
	}
}
