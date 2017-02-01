using System;
using Engine.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Unity.Network.Behaviours;
using PlayGen.Photon.Unity.Client;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Network
{
	// ReSharper disable once CheckNamespace
	public class PlayerCommands
	{
		static PlayerCommands()
		{
			DebugLog = true;
		}

		public static bool DebugLog { get; set; }

		private static void Log(string message)
		{
			if (DebugLog)
			{
				Debug.Log(message);
			}
		}

		public static Client PhotonClient { get; set; }

		public static void PickupItem(int itemId, int subsystemId)
		{
			Log($"Request PickupItem item: {itemId} at subsystem: {subsystemId}");
			//var requestPickupItemCommand = new RequestPickupItemCommand
			//{
			//	PlayerId = Director.Player.Id,
			//	ItemId = itemId,
			//	LocationId = subsystemId
			//};
			//IssueCommand(requestPickupItemCommand);

			// todo process locally too and resync later Director.RequestPickupItem(item.Id, subsystem.Id);
		}

		public static void Move(int subsystemId)
		{
			Log($"Request Move to subsystem: {subsystemId}");

			var requestMovePlayerCommand = new SetActorDestinationCommand()
			{
				PlayerId = Director.Player.Id,
				DestinationId = subsystemId
			};
			IssueCommand(requestMovePlayerCommand);

			// todo process locally too and resync laterDirector.RequestMovePlayer(destination.Id);
		}

		public static void DisownItem(int itemId)
		{
			Log($"Request Disown item: {itemId}");

			//var requestDropItemCommand = new RequestDropItemCommand()
			//{
			//	PlayerId = Director.Player.Id,
			//	ItemId = itemId
			//};
			//IssueCommand(requestDropItemCommand);

			// todo process locally too and resync laterDirector.RequestDropItem(item.Id);
		}

		public static void ActivateItem(int itemId)
		{
			Log($"Request Activate item: {itemId}");

			//var requestActivateItemCommand = new RequestActivateItemCommand()
			//{
			//	PlayerId = Director.Player.Id,
			//	ItemId = itemId
			//};
			//IssueCommand(requestActivateItemCommand);

			// todo process locally too and resync later Director.RequestActivateItem(item.Id);
		}

		public static void ActivateEnhancement(EnhancementBehaviour enhancement)
		{
			throw new NotImplementedException("Send Command to Simulation");
		}

		private static void IssueCommand(ICommand command)
		{
			PhotonClient.CurrentRoom.Messenger.SendMessage(new CommandMessage()
			{
				Command = command
			});
		}
	}
}