using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Controllers;

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