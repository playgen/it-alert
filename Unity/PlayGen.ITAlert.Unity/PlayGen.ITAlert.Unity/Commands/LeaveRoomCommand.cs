using GameWork.Core.Commands.Interfaces;

using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class LeaveRoomCommand : ICommand<LobbyController>
	{
		public void Execute(LobbyController parameter)
		{
			parameter.LeaveLobby();
		}
	}
}