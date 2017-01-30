using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Menu;
using PlayGen.ITAlert.Unity.GameStates.Menu.Settings;
using PlayGen.ITAlert.Unity.GameStates.Room.Error;
using PlayGen.ITAlert.Unity.GameStates.Room.Feedback;
using PlayGen.ITAlert.Unity.GameStates.Room.Initializing;
using PlayGen.ITAlert.Unity.GameStates.Room.Lobby;
using PlayGen.ITAlert.Unity.GameStates.Room.Paused;
using PlayGen.ITAlert.Unity.GameStates.Room.Playing;
using PlayGen.ITAlert.Unity.Network;
using PlayGen.ITAlert.Unity.Transitions;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Room
{
	public class StateControllerFactory
	{
		private readonly Client _photonClient;
		private readonly VoiceController _voiceController;

		public StateControllerBase ParentStateController { private get; set; }

		private readonly OnEventTransition _directorErrorTransition;

		public StateControllerFactory(Client photonClient, VoiceController voiceController)
		{
			_photonClient = photonClient;
			_voiceController = voiceController;
			_directorErrorTransition = new OnEventTransition(ErrorState.StateName);
			Director.SimulationError += () => _directorErrorTransition.ChangeState();
		}


		public TickStateController Create()
		{
			var lobbyState = CreateLobbyState(_photonClient, _voiceController);
			var initializingState = CreateInitializingState(_photonClient);
			var playingState = CreatePlayingState(_photonClient);
			var pausedState = CreatePausedState(_photonClient);
			var settingsState = CreateSettingsState(_photonClient);
			var feedbackState = CreateFeedbackState(_photonClient);
			var errorState = CreateErrorState();

			var stateController = new TickStateController(
				lobbyState,
				initializingState,
				playingState,
				pausedState,
				feedbackState,
				settingsState,
				errorState);

			stateController.SetParent(ParentStateController);

			return stateController;
		}

		private ErrorState CreateErrorState()
		{
			//var lobbyController = new LobbyController(_photonClient);
			var input = new ErrorStateInput();
			var state = new ErrorState(input, _photonClient);

			return state;
;		}

		private LobbyState CreateLobbyState(Client photonClient, VoiceController voiceController)
		{
			var lobbyController = new LobbyController(_photonClient);
			var input = new LobbyStateInput(lobbyController, photonClient);
			var state = new LobbyState(input, lobbyController, photonClient, voiceController);

			var initializingTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(InitializingMessage), InitializingState.StateName);
			var toMenuStateTransition = new OnEventTransition(MenuState.StateName);

			photonClient.LeftRoomEvent += toMenuStateTransition.ChangeState;

			state.AddTransitions(initializingTransition, toMenuStateTransition);

			return state;
		}

		private PausedState CreatePausedState(Client photonClient)
		{
			var input = new PausedStateInput();
			var state = new PausedState(input, photonClient);

			var onFeedbackStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(FeedbackMessage), FeedbackState.StateName);
			var toFromStateTransition = new ToFromStateTranstition();
			var toSettingsStateTransition = new OnEventTransition(SettingsState.StateName);
			var quitTransition = new QuitTransition();

			input.ContinueClickedEvent += toFromStateTransition.ChangeState;
			input.SettingsClickedEvent += toSettingsStateTransition.ChangeState;
			input.QuitClickedEvent += quitTransition.Quit;

			state.AddTransitions(onFeedbackStateSyncTransition, toFromStateTransition, toSettingsStateTransition, quitTransition, _directorErrorTransition);

			return state;
		}

		private PlayingState CreatePlayingState(Client photonClient)
		{
			var input = new PlayingStateInput();
			var state = new PlayingState(input, photonClient);

			var onFeedbackStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(FeedbackMessage), FeedbackState.StateName);
			var toPauseTransition = new OnEventTransition(PausedState.StateName);


			input.PauseClickedEvent += toPauseTransition.ChangeState;

			state.AddTransitions(onFeedbackStateSyncTransition, toPauseTransition, _directorErrorTransition);

			return state;
		}

		private InitializingState CreateInitializingState(Client photonClient)
		{
			var state = new InitializingState(photonClient);

			var onPlayingStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(PlayingMessage), PlayingState.StateName);
			state.AddTransitions(onPlayingStateSyncTransition, _directorErrorTransition);

			return state;
		}

		private SettingsState CreateSettingsState(Client photonClient)
		{
			var state = new SettingsState(new SettingsStateInput());

			var onFeedbackStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(FeedbackMessage), FeedbackState.StateName);

			state.AddTransitions(onFeedbackStateSyncTransition, _directorErrorTransition);

			return state; ;
		}

		private static FeedbackState CreateFeedbackState(Client photonClient)
		{
			var input = new FeedbackStateInput(photonClient);
			var state = new FeedbackState(input, photonClient);

			var onLobbyStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(LobbyMessage), LobbyState.StateName);

			state.AddTransitions(onLobbyStateSyncTransition);

			return state;
		}
	}
}