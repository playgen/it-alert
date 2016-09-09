﻿using UnityEngine;
using GameWork.States;
using PlayGen.SUGAR.Client;

public class ControllerBehaviour : MonoBehaviour
{

    private StateController<SequenceState> _stateController;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        // PASS SUGAR CLIENT TO EVERY STATE THAT NEEDS IT FOR PERSISTANCE
        var factory = new SUGARClient("http://localhost:62312/");

        _stateController = new StateController<SequenceState>(  
            new LoginState(new LoginStateInterface(), new LoginController(factory.Account), new RegisterController(factory.Account)), 
            new LoadingState(new LoadingStateInterface()),
            new MenuState(new MenuStateInterface(), new GameListController()),
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
