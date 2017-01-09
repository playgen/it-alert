using GameWork.Core.States.Interfaces;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network.Client;

namespace PlayGen.ITAlert.GameStates
{
	public class GameState : InputTickState
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

		public GameState(GameStateInput tickStateInput, Client client, LobbyController lobbyController, VoiceController voiceController) : base(tickStateInput)
		{
			_client = client;
			_lobbyController = lobbyController;
			_voiceController = voiceController;
		}

		public void Initialize()
		{
			_stateController = new TickStateController(new InitializingState(_client),
				new PlayingState(new PlayingTickableStateInput(), _client),
				new PausedState(new PausedStateInput(), _client),
				new FinalizingState(_client),
				new FeedbackState(_client),
				new SettingsState(new SettingsStateInput()));

			_stateController.SetParent(ParentStateController);
		}
		
		protected override void OnEnter()
		{
			_stateController.Initialize();
			_stateController.ChangeState(InitializingState.StateName);
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

		//public override void NextState()
		//{
		//	ChangeState(MainMenuState.StateName);
		//}

		//public override void PreviousState()
		//{
		//	ChangeState(LobbyState.StateName);
		//}
	}
}