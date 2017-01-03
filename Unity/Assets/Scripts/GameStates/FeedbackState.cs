using System;
using GameWork.Core.States;
using GameWork.Core.States.Controllers;
using PlayGen.ITAlert.GameStates.GameSubStates;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using PlayGen.ITAlert.GameStates;

public class FeedbackState : TickableSequenceState
{
    public const string StateName = "FeedbackState";

    private readonly Client _client;
    private readonly FeedbackStateInterface _interface;

	public override string Name
    {
        get { return StateName; }
    }

    public FeedbackState(Client client, FeedbackStateInterface @interface)
    {
        _client = client;
        _interface = @interface;
	}

    public override void Initialize()
    {
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
    }

    public override void Enter()
    {
        _interface.Enter();
        _interface.PopulateFeedback(_client.CurrentRoom.Players, _client.CurrentRoom.Player);
    }

    public override void Exit()
    {
        _interface.Exit();
    }

    public override void Tick(float deltaTime)
    {
		if (_interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();

			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
	}

    public override void NextState()
    {
        ChangeState(MenuState.StateName);
    }

    public override void PreviousState()
    {
        ChangeState(GameState.StateName);
    }
}