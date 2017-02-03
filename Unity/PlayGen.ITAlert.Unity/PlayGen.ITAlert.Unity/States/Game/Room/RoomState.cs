using GameWork.Core.States;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.States.Game.Room.Lobby;
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

		public RoomState(RoomStateInput tickStateInput, Client photonClient) 
			: base(tickStateInput)
		{
			_photonClient = photonClient;
			_controllerFactory = new RoomStateControllerFactory(photonClient);
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_controllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			_voiceController = new VoiceController(_photonClient);

			_stateController = _controllerFactory.Create();
			_stateController.Initialize();
			_stateController.EnterState(LobbyState.StateName);
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