using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.States.Game.Menu;
using PlayGen.ITAlert.Unity.States.Game.Room.Feedback;
using PlayGen.ITAlert.Unity.States.Game.Room.Initializing;
using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;
using PlayGen.ITAlert.Unity.States.Game.Room.Paused;
using PlayGen.ITAlert.Unity.States.Game.Room.Playing;
using PlayGen.ITAlert.Unity.States.Game.Settings;
using PlayGen.ITAlert.Unity.States.Game.SimulationSummary;
using PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked;

namespace PlayGen.ITAlert.Unity.States.Game.Room
{
	public class RoomStateControllerFactory
	{
		private readonly ITAlertPhotonClient _photonClient;

		public StateControllerBase ParentStateController { private get; set; }

		private readonly Director _director;
	    private readonly SimulationSummary.SimulationSummary _simulationSummary;

	    public RoomStateControllerFactory(Director director, ITAlertPhotonClient photonClient,
		    SimulationSummary.SimulationSummary simulationSummary)
		{
			_director = director;
			_photonClient = photonClient;
		    _simulationSummary = simulationSummary;
		}

		public TickStateController Create()
		{
			var lobbyState = CreateLobbyState(_photonClient);
			var initializingState = CreateInitializingState(_photonClient);
			var playingState = CreatePlayingState(_photonClient);
			var pausedState = CreatePausedState(_photonClient);
			var settingsState = CreateSettingsState(_photonClient);
			var feedbackState = CreateFeedbackState(_photonClient);

			var stateController = new TickStateController(
				lobbyState,
				initializingState,
				playingState,
				pausedState,
				feedbackState,
				settingsState);

			stateController.SetParent(ParentStateController);

			return stateController;
		}
		

		private LobbyState CreateLobbyState(ITAlertPhotonClient photonClient)
		{
			var lobbyController = new LobbyController(_photonClient);
			var lobbyStateInput = new LobbyStateInput(photonClient);
			var lobbyState = new LobbyState(lobbyStateInput, lobbyController, photonClient);

			var initializingTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(InitializingMessage), InitializingState.StateName);
			var toMenuStateTransition = new OnEventTransition(MenuState.StateName);

			photonClient.LeftRoomEvent += toMenuStateTransition.ChangeState;

			lobbyState.AddTransitions(initializingTransition, toMenuStateTransition);

			return lobbyState;
		}

		private PausedState CreatePausedState(ITAlertPhotonClient photonClient)
		{
			var pausedStateInput = new PausedStateInput(_director);
			var pausedState = new PausedState(pausedStateInput);

			var onFeedbackStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(FeedbackMessage), FeedbackState.StateName);
			var toPlayingStateTransition = new OnEventTransition(PlayingState.StateName);
			var toSettingsStateTransition = new OnEventTransition(SettingsState.StateName);
			var quitTransition = new OnEventTransition(MenuState.StateName);

			pausedStateInput.ContinueClickedEvent += toPlayingStateTransition.ChangeState;
			pausedStateInput.SettingsClickedEvent += toSettingsStateTransition.ChangeState;
			pausedStateInput.QuitClickedEvent += quitTransition.ChangeState;
			pausedStateInput.QuitClickedEvent += photonClient.CurrentRoom.Leave;

			pausedState.AddTransitions(onFeedbackStateSyncTransition, toPlayingStateTransition, toSettingsStateTransition, quitTransition);

			return pausedState;
		}

		private PlayingState CreatePlayingState(ITAlertPhotonClient photonClient)
		{
			var playingStateInput = new PlayingStateInput(_photonClient, _director);
			var playingState = new PlayingState(_director, playingStateInput, photonClient, _simulationSummary);

			var onFeedbackStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(FeedbackMessage), FeedbackState.StateName);
			var toFeedbackTransition = new OnEventTransition(FeedbackState.StateName);
			var toSimulationSummaryTransition = new OnEventTransition(SimulationSummaryState.StateName);
			var toMenuTransition = new OnEventTransition(MenuState.StateName);
			var toPauseTransition = new OnEventTransition(PausedState.StateName);

			playingStateInput.PauseClickedEvent += toPauseTransition.ChangeState;
			playingStateInput.EndGameContinueClickedEvent += toFeedbackTransition.ChangeState;
			playingStateInput.EndGameOnePlayerContinueClickedEvent += toSimulationSummaryTransition.ChangeState;
			playingStateInput.EndGameMaxOnePlayerContinueClickedEvent += toMenuTransition.ChangeState;

			playingState.AddTransitions(onFeedbackStateSyncTransition, toPauseTransition, toFeedbackTransition, toSimulationSummaryTransition, toMenuTransition);

			return playingState;
		}

		private InitializingState CreateInitializingState(ITAlertPhotonClient photonClient)
		{
			var input = new InitializingStateInput();
			var state = new InitializingState(_director, input, photonClient);

			var onPlayingStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(PlayingMessage), PlayingState.StateName);
			state.AddTransitions(onPlayingStateSyncTransition);

			return state;
		}

		private SettingsState CreateSettingsState(ITAlertPhotonClient photonClient)
		{
			var input = new SettingsStateInput();
			var state = new SettingsState(input);

			var previousStateTransition = new OnEventTransition(PausedState.StateName);
			var onFeedbackStateSyncTransition = new OnMessageTransition(photonClient, ITAlertChannel.GameState, typeof(FeedbackMessage), FeedbackState.StateName);

			input.BackClickedEvent += previousStateTransition.ChangeState;

			state.AddTransitions(onFeedbackStateSyncTransition, previousStateTransition);

			return state;
		}

		private FeedbackState CreateFeedbackState(ITAlertPhotonClient photonClient)
		{
			var input = new FeedbackStateInput(photonClient, _director);
			var state = new FeedbackState(input, photonClient);
            
		    var onLobbyStateSync = new OnMessageTransition(
		        photonClient,
		        ITAlertChannel.GameState,
		        typeof(LobbyMessage),
		        LobbyState.StateName);

            var onSendAndSimulationSummaryTransition = new OnEventTransition(
		        SimulationSummaryState.StateName,
		        () => _simulationSummary.HasData);

		    var onSendAndNoSimulationSummaryTransition = new OnEventTransition(
		        MenuState.StateName,
		        () => !_simulationSummary.HasData);

            input.FeedbackSendClickedEvent += onSendAndSimulationSummaryTransition.ChangeState;
		    input.FeedbackSendClickedEvent += onSendAndNoSimulationSummaryTransition.ChangeState;
            input.FeedbackSendClickedEvent += photonClient.CurrentRoom.Leave;

			state.AddTransitions(
			    onLobbyStateSync,
                onSendAndSimulationSummaryTransition,
			    onSendAndNoSimulationSummaryTransition);

			return state;
		}
	}
}