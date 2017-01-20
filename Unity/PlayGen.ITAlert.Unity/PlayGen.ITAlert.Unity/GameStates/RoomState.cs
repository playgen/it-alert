using GameWork.Core.States;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.Input;
using PlayGen.ITAlert.Unity.GameStates.RoomSubStates;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates
{
	public class RoomState : InputTickState
	{
		public const string StateName = "RoomState";
		
		private readonly VoiceController _voiceController;
		private readonly RoomSubStates.StateControllerFactory _controllerFactory;

		private TickStateController _stateController;

		public override string Name
		{
			get { return StateName; }
		}

		public RoomState(RoomStateInput tickStateInput, Client photonClient, VoiceController voiceController) : base(tickStateInput)
		{
			_voiceController = voiceController;
			_controllerFactory = new RoomSubStates.StateControllerFactory(photonClient, voiceController);
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