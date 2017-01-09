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

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ItemActivation : Engine.Systems.System, ITickableSystem
	{
		private IntentSystem _intentSystem;

		private readonly List<IItemActivationExtension> _itemActivationSystemComponents;
		
		private readonly ComponentMatcherGroup _activationMatcher;

		public ItemActivation(ComponentRegistry componentRegistry, EntityRegistry entityRegistry, SystemRegistry systemRegistry) 
			: base(componentRegistry, entityRegistry, systemRegistry)
		{
			_intentSystem = systemRegistry.GetSystem<IntentSystem>();

			_itemActivationSystemComponents = ModuleLoader.InstantiateTypesImplementing<IItemActivationExtension>().ToList();

			_activationMatcher = new ComponentMatcherGroup(new [] { typeof(Activation)});
			componentRegistry.RegisterMatcher(_activationMatcher);
		}

		public override void Tick(int currentTick)
		{
			foreach (var entity in _activationMatcher.MatchingEntities)
			{
				var activation = entity.GetComponent<Activation>();
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						// TODO: is activation pending?
						continue;
					case ActivationState.Activating:
						activation.SetState(ActivationState.Active);
						// TODO: confirm that this makes sense - process system extensions, then any components that are activatable
						ExecuteActivationSystemAction(ias => ias.OnActivating(entity, activation));
						ExecuteActivatableAction(entity, a => a.OnActivating(entity));
						break;
					case ActivationState.Deactivating:
						activation.SetState(ActivationState.NotActive);
						ExecuteActivationSystemAction(ias => ias.OnDeactivating(entity, activation));
						ExecuteActivatableAction(entity, a => a.OnDeactivating(entity));
						break;
					case ActivationState.Active:
						ExecuteActivationSystemAction(ias => ias.OnActive(entity, activation));
						ExecuteActivatableAction(entity, a => a.OnActive(entity));
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
		
		private void ExecuteActivationSystemAction(Action<IItemActivationExtension> action)
		{
			foreach (var activationSystem in _itemActivationSystemComponents)
			{
				action(activationSystem);
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
