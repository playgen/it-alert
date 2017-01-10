using System.Collections;
using GameWork.Core.States.Tick;
using UnityEngine;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.GameStates.Transitions;
using PlayGen.ITAlert.Network.Client;
using PlayGen.Photon.Unity;
using PlayGen.SUGAR.Client;

public class ControllerBehaviour : MonoBehaviour
{
	private const string GamePlugin = "RoomControllerPlugin";

	private TickStateController<TickState> _stateController;
	private string _gameVersion = "1";

	private Client _client;

	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);

		var clientBase = this.gameObject.AddComponent<PhotonClient>();
		clientBase.Initialize(_gameVersion, GamePlugin);

		_client = new Client(clientBase);

		StartCoroutine(ClientLoop());

		var factory = new SUGARClient("http://localhost:62312/");

		PlayerCommands.Client = _client;

		var popupController = new PopupController();
		PopupUtility.LogErrorEvent += popupController.ShowErrorPopup;
		PopupUtility.StartLoadingEvent += popupController.ShowLoadingPopup;
		PopupUtility.EndLoadingEvent += popupController.HideLoadingPopup;

		_stateController = CreateStates();
		_stateController.Initialize();
	}

	private TickStateController<TickState> CreateStates()
	{
		var voiceController = new VoiceController(_client);

		// Game
		var gameStateInput = new RoomStateInput(_client);
		var gameState = new RoomState(gameStateInput, _client, new LobbyController(_client), voiceController);

		// Menu
		var menuState = new MenuState(_client, voiceController);

		// Loading
		var loadingState = new LoadingState(new LoadingStateInput());
		loadingState.AddTransitions(new OnCompletedTransition(loadingState, LoginState.StateName));

		// Login
		var loginState = new LoginState();
		loginState.AddTransitions(new OnCompletedTransition(loginState, MenuState.StateName));

		var stateController = new TickStateController<TickState>(loadingState, loginState, menuState, gameState);

		gameState.ParentStateController = stateController;
		menuState.ParentStateController = stateController;

		return stateController;
	}

	private IEnumerator ClientLoop()
	{
		while (true)
		{
			Debug.Log("Current State: " + _client.ClientState);

			switch (_client.ClientState)
			{
				case Assets.Scripts.Network.Client.ClientState.Disconnected:
					_client.Connect();
					yield return new WaitForSeconds(0.1f);
					break;

				case Assets.Scripts.Network.Client.ClientState.Connecting:
					yield return new WaitForSeconds(0.5f);
					break;

				case Assets.Scripts.Network.Client.ClientState.Connected:
					yield return new WaitForSeconds(1.0f);
					break;
			}

			yield return new WaitForSeconds(1.0f);
;
		}
	}

	void Start ()
	{
		_stateController.ChangeState(LoadingState.StateName);
	}
	
	void Update ()
	{
		_stateController.Tick(Time.deltaTime);
	}
}
