using GameWork.Core.States;
using GameWork.Core.States.Tick;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Menu
{
	public class MenuState : TickState
	{
		public const string StateName = nameof(MenuState);

		public override string Name => StateName;

		private readonly MenuStateControllerFactory _controllerFactory;

		private TickStateController _stateController;
		

		public MenuState(Client photonClient)
		{
			_controllerFactory = new MenuStateControllerFactory(photonClient);
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