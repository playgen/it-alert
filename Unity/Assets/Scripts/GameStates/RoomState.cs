using GameWork.Core.States.Interfaces;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.GameStates.Transitions;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;

namespace PlayGen.ITAlert.GameStates
{
	public class RoomState : InputTickState
	{
		public const string StateName = "GameState";

		private TickStateController _stateController;
		private readonly Client _client;
		private readonly VoiceController _voiceController;
		private readonly LobbyController _lobbyController;

		public IStateController ParentStateController { private get; set; }

		public override string Name
		{
			get { return StateName; }
		}

		public RoomState(RoomStateInput tickStateInput, Client client, LobbyController lobbyController,
			VoiceController voiceController) : base(tickStateInput)
		{
			_client = client;
			_lobbyController = lobbyController;
			_voiceController = voiceController;
		}

		protected override void OnInitialize()
		{
			var lobbyState = CreateLobbyState(_client, _voiceController);

			var playingState = new PlayingState(new PlayingTickableStateInput(), _client);
			var onFeedbackStateSync = new OnMessageTransition(_client, Channels.GameState, typeof(FeedbackMessage),
				FeedbackState.StateName);
			playingState.AddTransitions(onFeedbackStateSync);

			var pausedState = new PausedState(new PausedStateInput(), _client);
			onFeedbackStateSync = new OnMessageTransition(_client, Channels.GameState, typeof(FeedbackMessage),
				FeedbackState.StateName);
			playingState.AddTransitions(onFeedbackStateSync);

			var settingsState = new SettingsState(new SettingsStateInput());
			onFeedbackStateSync = new OnMessageTransition(_client, Channels.GameState, typeof(FeedbackMessage),
				FeedbackState.StateName);
			playingState.AddTransitions(onFeedbackStateSync);

			var feedbackState = new FeedbackState(_client);
			var onLobbyStateSync = new OnMessageTransition(_client, Channels.GameState, typeof(LobbyMessage), LobbyState.StateName);
			feedbackState.AddTransitions(onLobbyStateSync);

			_stateController = new TickStateController(
				lobbyState,
				playingState,
				pausedState,
				feedbackState,
				settingsState);

			_stateController.SetParent(ParentStateController);
		}

		protected override void OnEnter()
		{
			_stateController.Initialize();
			_stateController.ChangeState(LobbyState.StateName);
		}

		protected override void OnExit()
		{
			_stateController.Terminate();
		}

		protected override void OnTick(float deltaTime)
		{
			_voiceController.HandleVoiceInput();

			if (_stateController.ActiveStateName != null)
			{
				_stateController.Tick(deltaTime);
			}
		}

		private LobbyState CreateLobbyState(Client client, VoiceController voiceController)
		{
			var lobbyController = new LobbyController(_client);
			var lobbyStateInput = new LobbyStateInput(lobbyController, client);

			var lobbyState = new LobbyState(lobbyStateInput, lobbyController, client, voiceController);

			var startGameTransition = new OnMessageTransition(client, Channels.GameState, typeof(PlayingMessage),
				PlayingState.StateName);
			lobbyState.AddTransitions(startGameTransition);

			return lobbyState;
		}
	}
}