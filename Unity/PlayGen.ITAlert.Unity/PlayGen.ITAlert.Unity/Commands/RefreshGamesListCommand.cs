using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.States.Game.Menu.GamesList;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class RefreshGamesListCommand : ICommand<GamesListController>
	{
		public void Execute(GamesListController parameter)
		{
			parameter.GetGamesList();
		}
	}
}