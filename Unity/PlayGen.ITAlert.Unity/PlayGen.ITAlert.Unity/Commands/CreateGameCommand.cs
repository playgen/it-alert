using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class CreateGameCommand : ICommand<CreateGameController>
	{
		private readonly CreateRoomSettings _createRoomSettings;
		
		public CreateGameCommand(CreateRoomSettings createRoomSettings)
		{
			_createRoomSettings = createRoomSettings;
		}

		public void Execute(CreateGameController parameter)
		{
			parameter.CreateGame(_createRoomSettings);
		}
	}
}