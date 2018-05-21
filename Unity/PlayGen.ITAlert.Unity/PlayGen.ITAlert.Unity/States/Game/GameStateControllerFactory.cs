using System.Collections.Generic;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.States.Game.Loading;
using PlayGen.ITAlert.Unity.States.Game.Menu;
using PlayGen.ITAlert.Unity.States.Game.Room;
using PlayGen.ITAlert.Unity.States.Game.SimulationSummary;
using PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked;

namespace PlayGen.ITAlert.Unity.States.Game
{
	public class GameStateControllerFactory
	{
		public StateControllerBase ParentStateController { get; set; }

		public TickStateController Create(ITAlertPhotonClient photonClient)
		{
		    var simulationSummary = new SimulationSummary.SimulationSummary();

			// Loading
			var loadingState = new LoadingState(new LoadingStateInput());
			loadingState.AddTransitions(new OnCompletedTransition(loadingState, LoginState.StateName));

			// Login
			var loginState = new LoginState();
			loginState.AddTransitions(new OnCompletedTransition(loginState, MenuState.StateName));

			// Menu
			var menuState = new MenuState(photonClient);

			// Room
			var roomStateInput = new RoomStateInput(photonClient);
			var roomState = new RoomState(roomStateInput, photonClient, simulationSummary);

			// Simulation Events Summary
            var simulationSummaryStateInput = new SimulationSummaryStateInput(simulationSummary);
		    var simulationSummaryState = new SimulationSummaryState(simulationSummaryStateInput, simulationSummary);

            var menuStateTransition = new OnEventTransition(MenuState.StateName);
		    simulationSummaryStateInput.ContinueClickedEvent += menuStateTransition.ChangeState;
            simulationSummaryState.AddTransitions(menuStateTransition);

            // Add states to controller
		    var stateController = new TickStateController(loadingState, loginState, menuState, roomState, simulationSummaryState);
		    stateController.SetParent(ParentStateController);

		    roomState.SetSubstateParentController(stateController);
		    menuState.SetSubstateParentController(stateController);

            return stateController;
		}
	}
}