using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class QuickGameCommand : ICommand<QuickGameController>
	{
		public void Execute(QuickGameController parameter)
		{
			parameter.QuickMatch();
		}
	}
}