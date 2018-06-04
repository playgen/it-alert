using System;

using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.States.Error;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Unity.Utilities.Localization;

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
			var gameState = new GameState();

			var hadExceptionTransition = new OnEventTransition(ErrorState.StateName);
			// exceptions thrown in the Unity Update loop dont propogate
			// TODO: firgure out how to catch unmity exceptions and trigger state transition
			// temporarily catch ui exception from the director ourselves
			gameState.Exception += GameExceptionHandler.OnException;
			GameExceptionHandler.HadUnignoredExceptionEvent += hadExceptionTransition.ChangeState;

			var disconnectedTransition = new OnEventTransition(ErrorState.StateName);
			gameState.Disconnected += disconnectedTransition.ChangeState;

			gameState.AddTransitions(hadExceptionTransition, disconnectedTransition);

			return gameState;
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
