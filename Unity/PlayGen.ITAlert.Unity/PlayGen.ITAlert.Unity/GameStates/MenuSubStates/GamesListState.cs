using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.MenuSubStates.Input;

namespace PlayGen.ITAlert.Unity.GameStates.MenuSubStates
{
	public class GamesListState : InputTickState
	{
		private readonly GamesListController _gameListController;
		private readonly JoinGameController _joinGameController;

		public const string StateName = "GameListState";

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
			ICommand command;
			if (CommandQueue.TryTakeFirstCommand(out command))
			{
				var refreshCommand = command as RefreshGamesListCommand;
				if (refreshCommand != null)
				{
					ExecuteRefresh(refreshCommand);
					return;
				}
				else
				{
					_lastRefresh += deltaTime;
					if (_lastRefresh >= _refreshInterval)
					{
						ExecuteRefresh(new RefreshGamesListCommand());
					}
				}

				var joinCommand = command as JoinGameCommand;
				if (joinCommand != null)
				{
					joinCommand.Execute(_joinGameController);
				}
			}
		}

		private void ExecuteRefresh(RefreshGamesListCommand refreshCommand)
		{
			refreshCommand.Execute(_gameListController);
			_lastRefresh = 0f;

		}
	}
}