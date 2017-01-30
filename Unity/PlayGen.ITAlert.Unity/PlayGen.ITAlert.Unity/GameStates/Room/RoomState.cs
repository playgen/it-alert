using GameWork.Core.States;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Room.Lobby;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Room
{
	public class RoomState : InputTickState
	{
		public const string StateName = "RoomState";
		public override string Name => StateName;
		
		private readonly VoiceController _voiceController;
		private readonly Room.StateControllerFactory _controllerFactory;

		private TickStateController _stateController;

		private readonly Client _photonClient;

		public RoomState(RoomStateInput tickStateInput, Client photonClient, VoiceController voiceController) 
			: base(tickStateInput)
		{
			_voiceController = voiceController;
			_controllerFactory = new Room.StateControllerFactory(photonClient, voiceController);
			_photonClient = photonClient;
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_controllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			_stateController = _controllerFactory.Create();
			_stateController.Initialize(LobbyState.StateName);
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
	}
}