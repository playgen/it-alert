using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Error;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.Photon.Unity.Client.Exceptions;

namespace PlayGen.ITAlert.Unity.States
{
	public class StateControllerFactory
	{
		public TickStateController Create()
		{
			var errorMessage = new GameErrorContainer();

			var errorState = CreateErrorState(errorMessage);
			var gameState = CreateGameState(errorMessage);

			var stateController = new TickStateController(errorState, gameState);
			
			return stateController;
		}

		private GameState CreateGameState(GameErrorContainer gameErrorContainer)
		{
			var state = new GameState(gameErrorContainer);

			var exceptionCaughtTransition = new OnExceptionEventTransition(ErrorState.StateName, typeof(ConnectionException));
			state.ExceptionEvent += exceptionCaughtTransition.ChangeState;
			// exceptions thrown in the Unity Update loop dont propogate
			// TODO: firgure out how to catch unmity exceptions and trigger state transition
			// temporarily catch ui exception from the director ourselves
			Director.ExceptionEvent += exceptionCaughtTransition.ChangeState;

			state.AddTransitions(exceptionCaughtTransition);

			return state;
		}

		private ErrorState CreateErrorState(GameErrorContainer gameErrorContainer)
		{
			var input = new ErrorStateInput(gameErrorContainer);
			var state = new ErrorState(input);

			var backClickedTransition = new OnEventTransition(GameState.StateName);
			input.BackClickedEvent += backClickedTransition.ChangeState;

			state.AddTransitions(backClickedTransition);

			return state;
		}
	}
}
