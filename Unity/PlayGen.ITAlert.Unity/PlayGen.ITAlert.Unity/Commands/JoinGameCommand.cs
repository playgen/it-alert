using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class JoinGameCommand : ICommand<JoinGameController>
	{
		private readonly string _name;

		public JoinGameCommand(string name)
		{
			_name = name;
		}

		public void Execute(JoinGameController parameter)
		{
			parameter.JoinGame(_name);
		}
	}
}