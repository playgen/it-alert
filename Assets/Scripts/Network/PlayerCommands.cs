using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
public class PlayerCommands
{
	public static void PickupItem(ItemBehaviour item, SubsystemBehaviour subsystem)
	{
		Director.RequestPickupItem(item.Id, subsystem.Id);
	}

	public static void Move(SubsystemBehaviour destination)
	{
		Director.RequestMovePlayer(destination.Id);
	}

	public static void DropItem(ItemBehaviour item)
	{
		Director.RequestDropItem(item.Id);
	}

	public static void ActivateItem(ItemBehaviour item)
	{
		Director.RequestActivateItem(item.Id);
	}

	public static void ActivateEnhancement(EnhancementBehaviour enhancement)
	{
		throw new NotImplementedException("Send Command to Simulation");
	}

}
