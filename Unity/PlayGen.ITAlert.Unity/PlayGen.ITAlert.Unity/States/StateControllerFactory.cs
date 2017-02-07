using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Error;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Exceptions;

namespace PlayGen.ITAlert.Unity.States
{
	public class StateControllerFactory
	{
		public TickStateController Create()
		{
			var errorState = CreateErrorState();
			var gameState = CreateGameState();

			var stateController = new TickStateController(errorState, gameState);
			
			return stateController;
		}

		private GameState CreateGameState()
		{
			var state = new GameState();

			var hadExceptionTransition = new OnEventTransition(ErrorState.StateName);
			// exceptions thrown in the Unity Update loop dont propogate
			// TODO: firgure out how to catch unmity exceptions and trigger state transition
			// temporarily catch ui exception from the director ourselves
			state.ExceptionEvent += GameExceptionHandler.OnException;
			GameExceptionHandler.HadUnignoredExceptionEvent += hadExceptionTransition.ChangeState;

			var disconnectedTransition = new OnEventTransition(state.Name);
			state.DisconnectedEvent += disconnectedTransition.ChangeState;

			state.AddTransitions(hadExceptionTransition, disconnectedTransition);

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
