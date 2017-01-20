using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class ReadyPlayerCommand : ICommand<LobbyController>
	{
		private readonly bool _ready;

		public ReadyPlayerCommand(bool ready)
		{
			_ready = ready;
		}

		public void Execute(LobbyController parameter)
		{
			if (_ready)
			{
				parameter.ReadyPlayer();
			}
			else
			{
				parameter.UnreadyPlayer();
			}
		}
	}
}