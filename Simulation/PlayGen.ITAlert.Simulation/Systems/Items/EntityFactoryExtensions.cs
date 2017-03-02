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
			Entity item;
			CurrentLocation currentLocation;
			Owner owner;
			if (entityFactoryProvider.TryCreateEntityFromArchetype(archetype, out item)
				&& item.TryGetComponent(out currentLocation)
				&& item.TryGetComponent(out owner))
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
