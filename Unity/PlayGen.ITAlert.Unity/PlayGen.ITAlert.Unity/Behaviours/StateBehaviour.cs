using GameWork.Core.States.Tick;
using UnityEngine;
using PlayGen.ITAlert.Unity.States;
using PlayGen.ITAlert.Unity.States.Game;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	[RequireComponent(typeof(DontDestroyOnLoad))]
	public class StateBehaviour : MonoBehaviour
	{
		private TickStateController _stateController;

		private void Awake()
		{
			var stateControllerFactory = new StateControllerFactory();
			_stateController = stateControllerFactory.Create();
		}

		private void Start()
		{
			_stateController.Initialize();
			_stateController.EnterState(GameState.StateName);
		}

		private void Update()
		{
			_stateController.Tick(Time.deltaTime);
		}

		private void OnApplicationQuit()
		{
			_stateController.Terminate();
		}
	}
}