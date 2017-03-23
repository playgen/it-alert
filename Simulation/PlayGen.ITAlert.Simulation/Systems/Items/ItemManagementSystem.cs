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
	// TODO: if we went for a more intent based approach, the pickup/drop item commands could enqueue the relevant intents which are then processed by this system 

	// TODO: it also occurs to me that there is little difference between intents and the command object themselves apart from that the prior has implicitly passed validation already

	//public class ItemManagementSystem : ITickableSystem
	//{
	//	private ComponentMatcherGroup<Owner> _itemTypeMatcher;

	//	public ItemManagementSystem(IMatcherProvider matcherProvider)
	//	{
	//		_itemTypeMatcher = matcherProvider.CreateMatcherGroup<Owner>();
	//	}

	//	public void Tick(int currentTick)
	//	{


	//	}

	//	#region command/ownership logic

	//	// TODO: decide if this is where these really belong

	//	public static bool CanChangeOwnership(Entity actor, Entity item)
	//	{
	//		Owner owner;
	//		return item.TryGetComponent(out owner)
	//				&& (owner.Value.HasValue == false || owner.Value == actor.Id);
	//	}

	//	/// <summary>
	//	/// Take (or release) ownership
	//	/// </summary>
	//	/// <param name="actor"></param>
	//	/// <param name="item"></param>
	//	/// <returns></returns>
	//	public static bool TryTakeOwnership(Entity actor, Entity item)
	//	{
	//		// TODO: test ownership (probably)
	//		if (CanChangeOwnership(actor, item))
	//		{
	//			var owner = item.GetComponent<Owner>();
	//			owner.Value = actor?.Id;
	//			return true;
	//		}
	//		return false;
	//	}

	//	#endregion
	//}
}
