using UnityEngine;
using GameWork.States;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network;
using PlayGen.SUGAR.Client;

public class ControllerBehaviour : MonoBehaviour
{
    private const string GamePlugin = "RoomControllerPlugin";

    private TickableStateController<TickableSequenceState> _stateController;
    private string _gameVersion = "1";


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        var clientBase = this.gameObject.AddComponent<Client>();
        clientBase.Initialize(_gameVersion, GamePlugin);

        var client = new ITAlertClient(clientBase);
        var factory = new SUGARClient("http://localhost:62312/");

        PlayerCommands.Client = client;

        var popupController = new PopupController();
        PopupUtility.LogErrorEvent += popupController.ShowErrorPopup;//
        PopupUtility.StartLoadingEvent += popupController.ShowLoadingPopup;
        PopupUtility.EndLoadingEvent += popupController.HideLoadingPopup;

        var joinGameController = new JoinGameController(client);
        var createGameController = new CreateGameController(client);
        var quickGameController = new QuickGameController(client, createGameController, 4);

        _stateController = new TickableStateController<TickableSequenceState>(  
            new LoginState(new LoginStateInterface(), new LoginController(factory.Account), new RegisterController(factory.Account), popupController, client), 
            new LoadingState(new LoadingStateInterface()),
            new MenuState(new MenuStateInterface(), quickGameController, client),
            new LobbyState(new LobbyStateInterface(), new LobbyController(client), client),
            new GamesListState(new GamesListStateInterface(), new GamesListController(client), joinGameController, client),
            new CreateGameState(new CreateGameStateInterface(), createGameController, client), 
            new SettingsState(new SettingsStateInterface()),
            new GameState(client)
            );
        _stateController.Initialize();
    }


    void Start ()
    {
        _stateController.SetState(LoadingState.StateName);
    }
    
    void Update ()
    {
        _stateController.Tick(Time.deltaTime);
    }

}
