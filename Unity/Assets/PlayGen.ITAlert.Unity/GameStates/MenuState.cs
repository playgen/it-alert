using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.GameStates.MenuSubStates;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates
{
	public class MenuState : TickState
	{
		public const string StateName = "MenuState";

		private readonly MenuSubStates.StateControllerFactory _controllerFactory;

		private TickStateController _stateController;
		
		public override string Name
		{
			get { return StateName; }
		}

		public MenuState(Client photonClient)
		{
			_controllerFactory = new MenuSubStates.StateControllerFactory(photonClient);
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