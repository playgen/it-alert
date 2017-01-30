using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Room.Lobby;

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