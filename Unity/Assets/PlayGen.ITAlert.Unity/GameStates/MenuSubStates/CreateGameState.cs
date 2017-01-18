using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.MenuSubStates.Input;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.MenuSubStates
{
	public class CreateGameState : InputTickState
	{
		private readonly CreateGameController _controller;
		private readonly Client _photonClient;
		public const string StateName = "CreateGameState";


		public CreateGameState(CreateGameStateInput input, CreateGameController controller) : base(input)
		{
			_controller = controller;
		}

		protected override void OnTick(float deltaTime)
		{
			ICommand command;
			if (CommandQueue.TryTakeFirstCommand(out command))
			{
				var createGameCommand = command as CreateGameCommand;
				if (createGameCommand != null)
				{
					createGameCommand.Execute(_controller);
					return;
				}
			}
		}

		public override string Name
		{
			get { return StateName; }
		}
	}
}