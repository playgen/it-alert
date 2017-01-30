using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Loading;
using PlayGen.ITAlert.Unity.GameStates.Menu;
using PlayGen.ITAlert.Unity.GameStates.Room;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates
{
	public class StateControllerFactory
	{
		private readonly Client _photonClient;

		public StateControllerFactory(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public TickStateController<TickState> Create()
		{
			var voiceController = new VoiceController(_photonClient);

			// Loading
			var loadingState = new LoadingState(new LoadingStateInput());
			loadingState.AddTransitions(new OnCompletedTransition(loadingState, LoginState.StateName));

			// Login
			var loginState = new LoginState();
			loginState.AddTransitions(new OnCompletedTransition(loginState, MenuState.StateName));

			// Menu
			var menuState = new MenuState(_photonClient);

			// Game
			var gameStateInput = new RoomStateInput(_photonClient);
			var gameState = new RoomState(gameStateInput, _photonClient, voiceController);

			var stateController = new TickStateController<TickState>(loadingState, loginState, menuState, gameState);

			gameState.SetSubstateParentController(stateController);
			menuState.SetSubstateParentController(stateController);

			return stateController;
		}
	}
}