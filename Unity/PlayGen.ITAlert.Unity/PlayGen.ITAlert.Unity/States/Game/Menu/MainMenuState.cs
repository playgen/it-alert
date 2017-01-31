using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Menu
{
	public class MainMenuState : InputTickState
	{
		public const string StateName = nameof(MainMenuState);

		public override string Name => StateName;

		private readonly QuickGameController _controller;
		
		public MainMenuState(MenuStateInput input, QuickGameController controller) : base(input)
		{
			_controller = controller;
		}

		protected override void OnTick(float deltaTime)
		{
			ICommand command;
			if (CommandQueue.TryTakeFirstCommand(out command))
			{
				var quickGameCommand = command as QuickGameCommand;
				if (quickGameCommand != null)
				{
					quickGameCommand.Execute(_controller);
				}
			}
		}
	}
}