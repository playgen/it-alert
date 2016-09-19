﻿using System;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Commands;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class PlayerCommands
{
	public static bool DebugLog { get; set; }

	private static void Log(string message)
	{
		if (DebugLog)
		{
			Debug.Log(message);
		}
	}

	public static ITAlertClient Client { get; set; }

	public static void PickupItem(int itemId, int subsystemId)
	{
		Log(string.Format("Request PickupItem item: {0} at subsystem: {1}", itemId, subsystemId));
		var requestPickupItemCommand = new RequestPickupItemCommand
		{
			PlayerId = Director.Player.Id,
			ItemId = itemId,
			LocationId = subsystemId
		};
		Client.SendGameCommand(requestPickupItemCommand);
		// todo process locally too and resync later Director.RequestPickupItem(item.Id, subsystem.Id);
	}

	public static void Move(int subsystemId)
	{
		Log(string.Format("Request Move to subsystem: {0}", subsystemId));

		var requestMovePlayerCommand = new RequestMovePlayerCommand()
		{
			PlayerId = Director.Player.Id,
			DestinationId = subsystemId
		};
		Client.SendGameCommand(requestMovePlayerCommand);
		// todo process locally too and resync laterDirector.RequestMovePlayer(destination.Id);
	}

	public static void DisownItem(int itemId)
	{
		Log(string.Format("Request Disown item: {0}", itemId));
		
		var requestDropItemCommand = new RequestDropItemCommand()
		{
			PlayerId = Director.Player.Id,
			ItemId = itemId
		};
		Client.SendGameCommand(requestDropItemCommand);
		// todo process locally too and resync laterDirector.RequestDropItem(item.Id);
	}

	public static void ActivateItem(int itemId)
	{
		Log(string.Format("Request Activate item: {0}", itemId));

		var requestActivateItemCommand = new RequestActivateItemCommand()
		{
			PlayerId = Director.Player.Id,
			ItemId = itemId
		};
		Client.SendGameCommand(requestActivateItemCommand);
		// todo process locally too and resync later Director.RequestActivateItem(item.Id);
	}

	public static void ActivateEnhancement(EnhancementBehaviour enhancement)
	{
		throw new NotImplementedException("Send Command to Simulation");
	}

}