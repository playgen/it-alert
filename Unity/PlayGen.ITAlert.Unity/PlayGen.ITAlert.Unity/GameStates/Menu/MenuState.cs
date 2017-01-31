using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Menu
{
	public class MenuState : TickState
	{
		public const string StateName = "MenuState";
		public override string Name => StateName;

		private readonly StateControllerFactory _controllerFactory;

		private TickStateController _stateController;
		

		public MenuState(Client photonClient)
		{
			_controllerFactory = new StateControllerFactory(photonClient);
		}

		public void SetSubstateParentController(StateControllerBase parentStateController)
		{
			_controllerFactory.ParentStateController = parentStateController;
		}

		protected override void OnEnter()
		{
			_stateController = _controllerFactory.Create();
			_stateController.Initialize(MainMenuState.StateName);
		}

		protected override void OnExit()
		{
			_stateController.Terminate();
		}

		protected override void OnTick(float deltaTime)
		{
			_stateController.Tick(deltaTime);
		}
	}
}