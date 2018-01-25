using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.GamesList
{
	public class GamesListState : InputTickState
	{
		private readonly GamesListController _gameListController;
		private readonly JoinGameController _joinGameController;

		public const string StateName = nameof(GamesListState);

		private float _refreshInterval = 5.0f;
		private float _lastRefresh;
		public override string Name => StateName;

		#region constructor

		public GamesListState(GamesListStateInput input, GamesListController gameListController,
			JoinGameController joinGameController) : base(input)
		{
			_gameListController = gameListController;
			_joinGameController = joinGameController;
		}

		#endregion

		protected override void OnTick(float deltaTime)
		{
			if (CommandQueue.TryTakeFirstCommand(out var command))
			{
				if (command is RefreshGamesListCommand refreshCommand)
				{
					ExecuteRefresh(refreshCommand);
					return;
				}
				_lastRefresh += deltaTime;
				if (_lastRefresh >= _refreshInterval)
				{
					ExecuteRefresh(new RefreshGamesListCommand());
				}

				var joinCommand = command as JoinGameCommand;
			    joinCommand?.Execute(_joinGameController);
			}
		}

		private void ExecuteRefresh(RefreshGamesListCommand refreshCommand)
		{
			refreshCommand.Execute(_gameListController);
			_lastRefresh = 0f;

		}
	}
}