using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Activation;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public class ItemManagement : Engine.Systems.System, ITickableSystem
	{
		private ComponentMatcherGroup _itemTypeMatcher;

		public ItemManagement(IComponentRegistry componentRegistry, IEntityRegistry entityRegistry)
			: base(componentRegistry, entityRegistry)
		{
			_itemTypeMatcher = componentRegistry.CreateMatcherGroup(new [] { typeof(Owner) });
		}

		public void Tick(int currentTick)
		{


		}



		#region command/ownership logic

		// TODO: decide if this is where these really belong

		public static bool CanChangeOwnership(Entity actor, Entity item)
		{
			Owner owner;
			return item.TryGetComponent(out owner)
					&& (owner.Value.HasValue == false || owner.Value == actor.Id);
		}

		/// <summary>
		/// Take (or release) ownership
		/// </summary>
		/// <param name="actor"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool TryTakeOwnership(Entity actor, Entity item)
		{
			// TODO: test ownership (probably)
			if (CanChangeOwnership(actor, item))
			{
				var owner = item.GetComponent<Owner>();
				owner.SetValue(actor?.Id ?? null);
				return true;
			}
			return false;
		}

		#endregion
	}
}
