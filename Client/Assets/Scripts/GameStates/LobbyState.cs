using System;
using System.Linq;
using GameWork.States;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyState : TickableSequenceState
{
    private readonly LobbyStateInterface _interface;
    private readonly LobbyController _controller;
    private readonly Client _client;
    private readonly VoiceController _voiceController;
    public const string StateName = "LobbyState";

    public LobbyState(LobbyStateInterface @interface, LobbyController controller, Client client, VoiceController voiceController)
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

        _client.CurrentRoom.PlayerReadyStatusChange += _interface.RefreshPlayerList;
        _client.PlayerRoomParticipationChange += _interface.RefreshPlayerList;
        _client.CurrentRoom.ChangeColorEvent += _interface.SetPlayerColors;
        _client.CurrentPlayerLeftRoomEvent += _interface.OnLeaveSuccess;
        _client.CurrentRoom.GameEnteredEvent += OnGameEntered;
        
        _interface.SetRoomMax(Convert.ToInt32(_client.CurrentRoom.RoomInfo.maxPlayers));
        _interface.SetRoomName(_client.CurrentRoom.RoomInfo.name);

        _interface.Enter();        
    }

    public override void Exit()
    {
        _client.PlayerRoomParticipationChange -= _interface.RefreshPlayerList;
        _client.CurrentRoom.PlayerReadyStatusChange -= _interface.RefreshPlayerList;
        _client.CurrentRoom.GameEnteredEvent -= OnGameEntered;

        _controller.RefreshSuccessEvent -= _interface.UpdatePlayerList;
        _controller.ReadySuccessEvent -= _interface.OnReadySucceeded;
        _client.CurrentPlayerLeftRoomEvent -= _interface.OnLeaveSuccess;
        _client.CurrentRoom.ChangeColorEvent -= _interface.SetPlayerColors;

        _interface.Exit();
    }

    private void OnGameEntered(ClientGame game)
    {
        NextState();
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
        get { return StateName; }
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

            var colorCommand = command as ChangePlayerColorCommand;
            if (colorCommand != null)
            {
                colorCommand.Execute(_controller);
                return;
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
        
        if (_client.CurrentRoom.IsMasterClient)
        {
            if (_client.CurrentRoom.PlayerReadyStatus != null && _client.CurrentRoom.PlayerReadyStatus.Values.All(v => v))
            {
                _client.CurrentRoom.StartGame(false);
            }
        }

        _interface.UpdateVoiceStatuses();
    }
}


