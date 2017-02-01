﻿using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.States.Error;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.Transitions;

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

			var exceptionCaughtTransition = new OnExceptionEventTransition(ErrorState.StateName);
			state.ExceptionEvent += exceptionCaughtTransition.ChangeState;

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