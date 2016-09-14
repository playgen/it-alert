using UnityEngine;
using GameWork.States;
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

        var joinGameController = new JoinGameController(client);

        _stateController = new TickableStateController<TickableSequenceState>(  
            new LoginState(new LoginStateInterface(), new LoginController(factory.Account), new RegisterController(factory.Account), client), 
            new LoadingState(new LoadingStateInterface()),
            new MenuState(new MenuStateInterface(), joinGameController, client),
            new LobbyState(new LobbyStateInterface(), new LobbyController(client), client),
            new GamesListState(new GamesListStateInterface(), new GamesListController(client), joinGameController, client),
            new CreateGameState(new CreateGameStateInterface(), new CreateGameController(client), client) 
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
