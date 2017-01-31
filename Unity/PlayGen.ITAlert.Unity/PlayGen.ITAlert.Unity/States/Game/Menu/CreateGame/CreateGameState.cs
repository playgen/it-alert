using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Menu.CreateGame
{
	public class CreateGameState : InputTickState
	{
		private readonly CreateGameController _controller;
		//private readonly Client _photonClient;

		public const string StateName = nameof(CreateGameState);
		public override string Name => StateName;

		#region constructor

		public CreateGameState(TickStateInput input, CreateGameController controller) 
			: base(input)
		{
			_controller = controller;
		}

		#endregion

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

	}
}