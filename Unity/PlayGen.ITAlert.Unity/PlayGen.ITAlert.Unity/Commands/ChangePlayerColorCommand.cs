using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;
using PlayGen.Photon.Players;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class ChangePlayerColorCommand : ICommand<LobbyController>
	{
		private readonly PlayerColour _playerColour;

		public ChangePlayerColorCommand(PlayerColour playerColour)
		{
			_playerColour = playerColour;
		}

		public void Execute(LobbyController parameter)
		{
			parameter.SetPlayerColour(_playerColour);
		}
	}
}