using System;
using System.Linq;
using GameWork.States;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network;
using UnityEngine;

public class LobbyState : TickableSequenceState
{
    private readonly LobbyStateInterface _interface;
    private readonly LobbyController _controller;
    private readonly ITAlertClient _client;
    private readonly VoiceController _voiceController;
    public const string stateName = "LobbyState";

    public LobbyState(LobbyStateInterface @interface, LobbyController controller, ITAlertClient client, VoiceController voiceController)
    {
        _interface = @interface;
        _controller = controller;
        _voiceController = voiceController;
        _client = client;
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
        _controller.ReadySuccessEvent += _interface.OnReadySucceeded;
        _controller.RefreshSuccessEvent += _interface.UpdatePlayerList;

        _client.PlayerReadyStatusChange += _interface.RefreshPlayerList;
        _client.PlayerRoomParticipationChange += _interface.RefreshPlayerList;
        _client.CurrentPlayerLeftRoomEvent += _interface.OnLeaveSuccess;
        _client.GameEnteredEvent += NextState;
        _interface.SetRoomMax(Convert.ToInt32(_client.CurrentRoom.maxPlayers));
        _interface.SetRoomName(_client.CurrentRoom.name);
        _interface.Enter();
    }

    public override void Exit()
    {
        _client.PlayerRoomParticipationChange -= _interface.RefreshPlayerList;
        _client.PlayerReadyStatusChange -= _interface.RefreshPlayerList;
        _client.GameEnteredEvent -= NextState;

        _controller.RefreshSuccessEvent -= _interface.UpdatePlayerList;
        _controller.ReadySuccessEvent -= _interface.OnReadySucceeded;
        _client.CurrentPlayerLeftRoomEvent -= _interface.OnLeaveSuccess;
        _interface.Exit();
    }

    public override void NextState()
    {
        ChangeState(GameState.StateName);
    }

    public override void PreviousState()
    {
        
       ChangeState(MenuState.StateName);
    }

    public override string Name
    {
        get { return stateName; }
    }

    public override void Tick(float deltaTime)
    {
        _voiceController.HandleVoiceInput();    

        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            var leaveCommand = command as LeaveRoomCommand;
            if (leaveCommand != null)
            {
                leaveCommand.Execute(_controller);
                return;
            }

            var readyCommand = command as ReadyPlayerCommand;
            if (readyCommand != null)
            {
                readyCommand.Execute(_controller);
                return;
            }

            var refreshCommand = command as RefreshPlayerListCommand;
            if (refreshCommand != null)
            {
                refreshCommand.Execute(_controller);
                return;
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }

        if (_client.IsMasterClient)
        {
            if (_client.PlayerReadyStatus.Values.All(v => v))
            {
                _client.StartGame(false);
            }
        }
    }
}


