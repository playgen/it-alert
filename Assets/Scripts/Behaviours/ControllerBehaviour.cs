using UnityEngine;
using GameWork.States;

public class ControllerBehaviour : MonoBehaviour
{

    private StateController _stateController;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        // PASS SUGAR CLIENT TO EVERY STATE THAT NEEDS IT FOR PERSISTANCE

        _stateController = new StateController(  
            new LoginState(new LoginStateInterface(), new LoginController()), 
            new LoadingState(new LoadingStateInterface()),
            new MenuState(new MenuStateInterface()),
            new LobbyState(new LobbyStateInterface(), new ReadyPlayerController()) 
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
