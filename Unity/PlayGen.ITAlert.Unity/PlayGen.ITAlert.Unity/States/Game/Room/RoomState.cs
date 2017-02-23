using System;
using GameWork.Core.States;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Messages.Error;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room
{
	public class RoomState : InputTickState
	{
		public const string StateName = "RoomState";
		public override string Name => StateName;
		
		private readonly RoomStateControllerFactory _controllerFactory;
		private readonly Client _photonClient;

		private TickStateController _stateController;
		private VoiceController _voiceController;

		public RoomState(RoomStateInput roomStateInput, Client photonClient) 
			: base(roomStateInput)
		{
			_photonClient = photonClient;
			_controllerFactory = new RoomStateControllerFactory(roomStateInput.Director, photonClient);
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_controllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			_photonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.Error, ProcessErrorMessage);

			_voiceController = new VoiceController(_photonClient);

			_stateController = _controllerFactory.Create();
			_stateController.Initialize();

			if (!GameExceptionHandler.HasException)
			{
				_stateController.EnterState(LobbyState.StateName);
			}
		}

		private void ProcessErrorMessage(Message message)
		{
			var errorMessage = message as ErrorMessage;
			if (errorMessage != null)
			{
				throw new Exception(errorMessage.Message);
			}
		}

		protected override void OnExit()
		{
			_photonClient.CurrentRoom?.Leave();

			// CurrentRoom and messenger would have been destroyed by this point so no need to unsubscribe
			_stateController.Terminate();

			LoadingUtility.LoadingSpinner.StopSpinner();
		}

		protected override void OnTick(float deltaTime)
		{
			_voiceController.HandleVoiceInput();

			if (_stateController.ActiveStateName != null)
			{
				_stateController.Tick(deltaTime);
			}
		}
	}
}