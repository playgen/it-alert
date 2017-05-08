using System.Collections;
using GameWork.Core.States.Tick;
using GameWork.Unity.Engine.Components;
using Newtonsoft.Json;
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
		private static string ConfigPath => Application.streamingAssetsPath + "/Photon.config.json";

		private bool _loaded;

		private IEnumerator LoadPhotonConfig(string path)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64)
			{
				path = "file:///" + path;
			}
			var www = new WWW(path);
			yield return www;

			var config = JsonConvert.DeserializeObject<ServerSettings>(www.text);
			PhotonNetwork.PhotonServerSettings = config;

			_stateController.EnterState(GameState.StateName);
			_loaded = true;
		}

		private TickStateController _stateController;

		private void Awake()
		{
			LogProxy.LogLevel = LogType.Warning;

			GameExceptionHandler.AddExceptionTypeToIgnore(typeof(ConnectionException));

			var stateControllerFactory = new StateControllerFactory();
			_stateController = stateControllerFactory.Create();
		}

		private void Start()
		{
			StartCoroutine(LoadPhotonConfig(ConfigPath));

			_stateController.Initialize();
		}

		private void Update()
		{
			if (_loaded)
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
		}

		private void OnApplicationQuit()
		{
			_stateController.Terminate();
		}
	}
}