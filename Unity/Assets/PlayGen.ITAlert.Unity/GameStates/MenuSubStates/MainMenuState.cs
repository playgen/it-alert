using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Input;

namespace PlayGen.ITAlert.Unity.GameStates.MenuSubStates
{
	public class MainMenuState : InputTickState
	{
		private readonly QuickGameController _controller;
		public const string StateName = "MainMenuState";

		public override string Name
		{
			get { return StateName; }
		}

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