using System;
using GameWork.Core.States;
using GameWork.Core.States.Controllers;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.GameStates
{
	public class GameState : TickableSequenceState
	{
		public const string StateName = "GameState";

		private readonly TickableStateController _stateController;
		private readonly Client _client;
		private readonly VoiceController _voiceController;
		private readonly GameStateInterface _interface;
		private readonly LobbyController _lobbyController;

		public override string Name
		{
			get { return StateName; }
		}

		public GameState(Client client, GameStateInterface @interface, LobbyController lobbyController, VoiceController voiceController)
		{
			_client = client;
			_interface = @interface;
			_stateController = new TickableStateController(new InitializingState(_client),
				new PlayingState(new PlayingStateInterface(), _client),
				new PausedState(new PausedStateInterface(), _client),
				new FinalizingState(_client),
				new SettingsState(new SettingsStateInterface()));

			_stateController.ChangeParentStateEvent += ChangeState;
			_lobbyController = lobbyController;
			_voiceController = voiceController;
		}

		public override void Terminate()
		{
			_stateController.ChangeParentStateEvent -= ChangeState;
		}

		public override void Enter()
		{
			_client.CurrentRoom.Messenger.Subscribe((int)PlayGen.ITAlert.Photon.Messages.Channels.Game, ProcessGameMessage);

			_stateController.Initialize();
			_stateController.ChangeState(InitializingState.StateName);
			_interface.Initialize();
			_interface.SetPlayerColors(_client.CurrentRoom.Players);
			_interface.PopulateChatPanel(_client.CurrentRoom.Players);
		}

		public override void Exit()
		{
			_client.CurrentRoom.Messenger.Unsubscribe((int)PlayGen.ITAlert.Photon.Messages.Channels.Game, ProcessGameMessage);

			_interface.Exit();
			_stateController.Terminate();
		}

		public override void Tick(float deltaTime)
		{
			_voiceController.HandleVoiceInput();

			if (_stateController.ActiveState != null)
			{
				_stateController.Tick(deltaTime);
			}

			// todo have internal states controlled by messaging in here

			// todo remove commented out section
			//{
			//	case Assets.Scripts.Network.Client.GameStates.Initializing:
			//		if (_stateController.ActiveState != InitializingState.StateName)
			//		{
			//			_stateController.ChangeState(InitializingState.StateName);
			//		}
			//		else
			//		{
			//			_stateController.Tick(deltaTime);
			//		}
			//		break;

			//	case Assets.Scripts.Network.Client.GameStates.Playing:
			//		if (_stateController.ActiveState == InitializingState.StateName || _stateController.ActiveState == FinalizingState.StateName)
			//		{
			//			_stateController.ChangeState(PlayingState.StateName);
			//		}
			//		else
			//		{
			//			_stateController.Tick(deltaTime);
			//		}
			//		break;

			//	case Assets.Scripts.Network.Client.GameStates.Finalizing:
			//		if (_stateController.ActiveState != FinalizingState.StateName)
			//		{
			//			_stateController.ChangeState(FinalizingState.StateName);
			//		}
			//		else
			//		{
			//			_stateController.Tick(deltaTime);
			//		}
			//		break;
			//}

			_interface.UpdateChatPanel();
		}

		public override void NextState()
		{
			ChangeState(MainMenuState.StateName);
		}

		public override void PreviousState()
		{
			ChangeState(LobbyState.StateName);
		}
		
		private void ProcessGameMessage(Message message)
		{
			var gameEndedMessage = message as GameEndedMessage;
			if (gameEndedMessage != null)
			{
				ChangeState(MainMenuState.StateName);
			}
		}
	}
}