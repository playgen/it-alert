using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.GameStates.Error;
using PlayGen.ITAlert.Unity.GameStates.Game;
using PlayGen.ITAlert.Unity.Transitions;

namespace PlayGen.ITAlert.Unity.States
{
	public class StateControllerFactory
	{
		public TickStateController Create()
		{
			var errorState = CreateErrorState();
			var ganeState = CreateGameState();

			var stateController = new TickStateController(errorState, ganeState);
			
			return stateController;
		}

		private GameState CreateGameState()
		{
			var state = new GameState();

			var exceptionCaughtTransition = new OnExceptionEventTransition(ErrorState.StateName);
			state.ExceptionEvent += exceptionCaughtTransition.ChangeState;

			state.AddTransitions(exceptionCaughtTransition);

			return state;
		}

		private ErrorState CreateErrorState()
		{
			var input = new ErrorStateInput();
			var state = new ErrorState(input);

			var backClickedTransition = new OnEventTransition(GameState.StateName);
			input.BackClickedEvent += backClickedTransition.ChangeState;

			state.AddTransitions(backClickedTransition);

			return state;
		}
	}
}
