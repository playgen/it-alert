﻿using GameWork.Core.States;
using GameWork.Core.States.Tick;

using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;

namespace PlayGen.ITAlert.Unity.States.Game.Menu
{
	public class MenuState : TickState
	{
		public const string StateName = nameof(MenuState);

		public override string Name => StateName;

		private readonly MenuStateControllerFactory _controllerFactory;

		private TickStateController _stateController;
		

		public MenuState(ITAlertPhotonClient photonClient)
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
			_stateController.Initialize();

			if (!GameExceptionHandler.HasException)
			{
				_stateController.EnterState(MainMenuState.StateName);
			}
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