using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class CreateGameCommand : ICommand<CreateGameController>
	{
		private readonly string _gameName;
		private readonly int _maxPlayers;

		public CreateGameCommand(string gameName, int maxPlayers)
		{
			_gameName = gameName;
			_maxPlayers = maxPlayers;
		}

		public void Execute(CreateGameController parameter)
		{
			parameter.CreateGame(_gameName, _maxPlayers);
		}
	}
}