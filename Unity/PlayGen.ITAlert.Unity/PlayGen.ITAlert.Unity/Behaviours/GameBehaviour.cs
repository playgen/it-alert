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
using AsyncLogger = GameWork.Unity.Logging.AsyncLogger;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	[RequireComponent(typeof(DontDestroyOnLoad))]
	public class GameBehaviour : MonoBehaviour
	{
	    public static DebugConfig DebugConfig { get; private set; }

	    private static string DebugConfigPath => Application.streamingAssetsPath + "/Debug.config.json";
        private static string PhotonConfigPath => Application.streamingAssetsPath + "/Photon.config.json";
        private static readonly AsyncLogger Logger = new AsyncLogger();

        private bool _loaded;
	    private bool _isDisposed;

        static GameBehaviour()
	    {
	        LogProxy.SetLogger(Logger);
        }

        private IEnumerator LoadConfigs()
        {
            // Game Config
            var debugConfigTextLoader = CreateTextLoader(DebugConfigPath);
            yield return debugConfigTextLoader;

	        DebugConfig = new DebugConfig();
	        DebugConfig = JsonConvert.DeserializeObject<DebugConfig>(debugConfigTextLoader.text);
            LogProxy.LogLevel = DebugConfig.LogLevel;

            // Photon Config
            var photonConfigTextLoader = CreateTextLoader(PhotonConfigPath);
            yield return photonConfigTextLoader;

            var photonConfig = JsonConvert.DeserializeObject<ServerSettings>(photonConfigTextLoader.text);
			PhotonNetwork.PhotonServerSettings = photonConfig;
            
            _stateController.EnterState(GameState.StateName);
			_loaded = true;
		}

	    private WWW CreateTextLoader(string path)
	    {
	        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64)
	        {
	            path = "file:///" + path;
	        }

	        return new WWW(path);
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
			StartCoroutine(LoadConfigs());
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
		    if (!_isDisposed)
		    {
		        Application.CancelQuit();

		        _stateController.Terminate();
		        LogProxy.Info("Game Quit Gracefully.");
                Logger.Flush();

                _isDisposed = true;

		        Application.Quit();
            }
		}
	}
}