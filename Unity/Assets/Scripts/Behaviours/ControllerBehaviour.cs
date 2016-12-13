using System.Collections;
using UnityEngine;
using GameWork.Core.States;
using GameWork.Core.States.Controllers;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network.Client;
using PlayGen.SUGAR.Client;

public class ControllerBehaviour : MonoBehaviour
{
    private const string GamePlugin = "RoomControllerPlugin";

    private TickableStateController<TickableSequenceState> _stateController;
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
        PopupUtility.LogErrorEvent += popupController.ShowErrorPopup;//
        PopupUtility.StartLoadingEvent += popupController.ShowLoadingPopup;
        PopupUtility.EndLoadingEvent += popupController.HideLoadingPopup;
        //PopupUtility.ColorPickerEvent += popupController.ShowColorPickerPopup;

        var joinGameController = new JoinGameController(_client);
        var createGameController = new CreateGameController(_client);
        var quickGameController = new QuickGameController(_client, createGameController, 4);
        var voiceController = new VoiceController(_client);

        _stateController = new TickableStateController<TickableSequenceState>(  
            new LoginState(), 
            new LoadingState(new LoadingStateInterface()),
            new MenuState(new MenuStateInterface(), quickGameController, _client),
            new LobbyState(new LobbyStateInterface(), new LobbyController(_client), _client, voiceController),
            new GamesListState(new GamesListStateInterface(), new GamesListController(_client), joinGameController, _client),
            new CreateGameState(new CreateGameStateInterface(), createGameController, _client), 
            new SettingsState(new SettingsStateInterface()),
            new GameState(_client, new GameStateInterface(), new LobbyController(_client), voiceController)
            );
        _stateController.Initialize();
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
