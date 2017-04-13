using GameWork.Core.States.Tick;
using GameWork.Unity.Engine.Components;
using UnityEngine;
using PlayGen.ITAlert.Unity.States;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Exceptions;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	[RequireComponent(typeof(DontDestroyOnLoad))]
	public class GameBehaviour : MonoBehaviour
	{
		private TickStateController _stateController;

		private void Awake()
		{
			LogProxy.LogLevel = LogType.Log;

			GameExceptionHandler.AddExceptionTypeToIgnore(typeof(ConnectionException));

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
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				if (Input.GetKeyDown(KeyCode.Insert))
				{
					Application.CaptureScreenshot(System.DateTime.UtcNow.ToFileTimeUtc() + ".png");
				}
			}
		}

		private void OnApplicationQuit()
		{
			_stateController.Terminate();
		}
	}
}