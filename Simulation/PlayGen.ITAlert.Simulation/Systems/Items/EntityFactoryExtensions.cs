using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Components.Common;
using PlayGen.ITAlert.Simulation.Components.Items;

namespace PlayGen.ITAlert.Simulation.Systems.Items
{
	public static class EntityFactoryExtensions
	{
		public static bool TryCreateItem(this IEntityFactoryProvider entityFactoryProvider, 
			string archetype,
			int? currentLocationId, 
			int? ownerId,
			out ComponentEntityTuple<CurrentLocation, Owner> entityTuple)
		{
			if (entityFactoryProvider.TryCreateEntityFromArchetype(archetype, out var item)
				&& item.TryGetComponent<CurrentLocation>(out var currentLocation)
				&& item.TryGetComponent<Owner>(out var owner))
			{
				currentLocation.Value = currentLocationId;
				owner.Value = ownerId;
				entityTuple = new ComponentEntityTuple<CurrentLocation, Owner>(item, currentLocation, owner);
				return true;
			}
			item?.Dispose();
			entityTuple = null;
			return false;
		}
	}
}
