using UnityEngine;
using GameWork.States;
using PlayGen.ITAlert.Network;
using PlayGen.SUGAR.Client;

public class ControllerBehaviour : MonoBehaviour
{
    private const string GamePlugin = "GameControllerPlugin";

    private StateController<SequenceState> _stateController;
    private string _gameVersion = "1";


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        var clientBase = this.gameObject.AddComponent<Client>();
        clientBase.Initialize(_gameVersion, GamePlugin);

        var client = new ITAlertClient(clientBase);
        var factory = new SUGARClient("http://localhost:62312/");

        _stateController = new StateController<SequenceState>(  
            new LoginState(new LoginStateInterface(), new LoginController(factory.Account), new RegisterController(factory.Account)), 
            new LoadingState(new LoadingStateInterface()),
            new MenuState(new MenuStateInterface(), new CreateGameController(client)),
            new LobbyState(new LobbyStateInterface(), new ReadyPlayerController()),
            new GamesListState(new GamesListStateInterface(), new GamesListController(client)) 
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
