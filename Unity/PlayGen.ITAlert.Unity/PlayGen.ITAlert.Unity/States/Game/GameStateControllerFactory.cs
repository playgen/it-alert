using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.States.Game.Loading;
using PlayGen.ITAlert.Unity.States.Game.Menu;
using PlayGen.ITAlert.Unity.States.Game.Room;
using PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game
{
	public class GameStateControllerFactory
	{
		public StateControllerBase ParentStateController { get; set; }

		public TickStateController Create(Client photonClient)
		{
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
			var roomState = new RoomState(roomStateInput, photonClient);

			var stateController = new TickStateController(loadingState, loginState, menuState, roomState);
			stateController.SetParent(ParentStateController);

			roomState.SetSubstateParentController(stateController);
			menuState.SetSubstateParentController(stateController);

			return stateController;
		}
	}
}