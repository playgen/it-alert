using Engine.Commands;

using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Unity.Photon;

namespace PlayGen.ITAlert.Unity.Debug
{
	public class DebugCommands
	{
		static DebugCommands()
		{
			DebugLog = true;
		}

		public static bool DebugLog { get; set; }

		private static void Log(string message)
		{
			if (DebugLog)
			{
				LogProxy.Info(message);
			}
		}

		public static ITAlertPhotonClient PhotonClient { get; set; }

		public static void ChangePlayerSpeedOffset(decimal delta)
		{
			Log($"Change Player Speed Offset: {delta}");
			var changeSpeedCommand = new SetMovementSpeedCommand
			{
				Delta = delta,
				Category = ITAlert.Simulation.Components.Movement.MovementOffsetCategory.Player
			};
			IssueCommand(changeSpeedCommand);
		}

		private static void IssueCommand(ICommand command)
		{
			PhotonClient.CurrentRoom.Messenger.SendMessage(new CommandMessage
																{
				Command = command
			});
		}
	}
}
