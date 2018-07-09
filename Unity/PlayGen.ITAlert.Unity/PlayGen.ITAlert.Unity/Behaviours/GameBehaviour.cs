using System.Collections;
using GameWork.Core.Logging.Loggers;
using GameWork.Core.States.Tick;
using GameWork.Unity.Engine.Components;
using Newtonsoft.Json;
using UnityEngine;
using PlayGen.ITAlert.Unity.States;
using PlayGen.ITAlert.Unity.States.Game;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Exceptions;
using ThreadedLogger = GameWork.Unity.Logging.ThreadedLogger;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	[RequireComponent(typeof(DontDestroyOnLoad))]
	public class GameBehaviour : MonoBehaviour
	{
		private static string ConfigPath => Application.streamingAssetsPath + "/Photon.config.json";

		private bool _loaded;
        private static string PhotonConfigPath => Application.streamingAssetsPath + "/Photon.config.json";
	    private static string GameConfigPath => Application.streamingAssetsPath + "/Game.config.json";
        private static readonly ThreadedLogger Logger = new ThreadedLogger();

		private IEnumerator LoadPhotonConfig(string path)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64)
			{
				path = "file:///" + path;
			}
			var www = new WWW(path);
			yield return www;
        static GameBehaviour()
	    {
	        LogProxy.SetLogger(Logger);
        }

			var config = JsonConvert.DeserializeObject<ServerSettings>(www.text);
			PhotonNetwork.PhotonServerSettings = config;

			_stateController.EnterState(GameState.StateName);
			_loaded = true;
		}

		private TickStateController _stateController;

		private void Awake()
		{
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
						ScreenCapture.CaptureScreenshot(System.DateTime.UtcNow.ToFileTimeUtc() + ".png");
					}
				}
			}
		}

		private void OnApplicationQuit()
		{
			_stateController.Terminate();
            LogProxy.Info("Game Gracefully Terminated.");
		}
	}
}