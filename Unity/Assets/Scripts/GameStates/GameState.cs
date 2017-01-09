using GameWork.Core.States;
using GameWork.Core.States.Interfaces;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network.Client;

namespace PlayGen.ITAlert.GameStates
{
	public class GameState : TickState
	{
		public const string StateName = "GameState";

		private TickStateController _stateController;
		private readonly Client _client;
		private readonly VoiceController _voiceController;
		private readonly GameStateInterface _interface;
		private readonly LobbyController _lobbyController;

		public IStateController ParentStateController { private get; set; }

		public override string Name
		{
			get { return StateName; }
		}

		public GameState(Client client, GameStateInterface @interface, LobbyController lobbyController, VoiceController voiceController)
		{
			_client = client;
			_interface = @interface;
			_lobbyController = lobbyController;
			_voiceController = voiceController;
		}

		public void Initialize()
		{
			_stateController = new TickStateController(ParentStateController,
				new InitializingState(_client),
				new PlayingState(new PlayingTickableStateInterface(), _client),
				new PausedState(new PausedStateInterface(), _client),
				new FinalizingState(_client),
				new FeedbackState(_client),
				new SettingsState(new SettingsStateInterface()));
		}
		
		public override void Enter()
		{
			_stateController.Initialize();
			_stateController.ChangeState(InitializingState.StateName);
			_interface.Initialize();
			_interface.SetPlayerColors(_client.CurrentRoom.Players);
			_interface.PopulateChatPanel(_client.CurrentRoom.Players);
		}

		public override void Exit()
		{
			_interface.Exit();
			_stateController.Terminate();
		}

		public override void Tick(float deltaTime)
		{
			_voiceController.HandleVoiceInput();

			if (_stateController.ActiveStateName != null)
			{
				_stateController.Tick(deltaTime);
			}

			_interface.UpdateChatPanel();
		}

		// todo refactor states
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