using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame
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
			if (CommandQueue.TryTakeFirstCommand(out var command))
			{
				var createGameCommand = command as CreateGameCommand;
			    createGameCommand?.Execute(_controller);
			}
		}

	}
}