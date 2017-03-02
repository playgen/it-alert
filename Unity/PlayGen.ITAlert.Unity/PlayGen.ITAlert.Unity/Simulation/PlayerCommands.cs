using System;
using Engine.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Movement;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.Photon.Unity.Client;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation
{
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

		public static Director Director { get; set; }

		public static void PickupItem(int itemId)
		{
			Log($"Request PickupItem item: {itemId}");
			var pickupItemCommand = new PickupItemCommand
			{
				PlayerId = Director.Player.Id,
				ItemId = itemId
			};
			IssueCommand(pickupItemCommand);
		}

		public static void DropItem(int itemId, int containerIndex)
		{
			Log($"Request PickupItem item: {itemId}");
			var pickupItemCommand = new DropItemCommand()
			{
				PlayerId = Director.Player.Id,
				ItemId = itemId,
				ContainerId = containerIndex,
			};
			IssueCommand(pickupItemCommand);
		}

		public static void Continue()
		{
			Log($"Request tutorial continue");

			var continueCommand = new ContinueCommand();
			IssueCommand(continueCommand);
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

		public static void ActivateItem(int itemId)
		{
			Log($"Request Activate item: {itemId}");

			var activateItemCommand = new ActivateItemCommand()
			{
				PlayerId = Director.Player.Id,
				ItemId = itemId
			};
			IssueCommand(activateItemCommand);
		}

		public static void HaltAndFire()
		{
			Log("Halt and Fire");

			IssueCommand(new HaltAndCatchFireCommand());
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