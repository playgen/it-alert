using System;
using GameWork.States;
using PlayGen.ITAlert.Network;

public class LobbyState : TickableSequenceState
{
    private readonly LobbyStateInterface _interface;
    private readonly LobbyController _controller;
    private readonly ITAlertClient _client;
    public const string stateName = "LobbyState";

    public LobbyState(LobbyStateInterface @interface, LobbyController controller, ITAlertClient client)
    {
        _interface = @interface;
        _controller = controller;
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
        _interface.SetRoomMax(Convert.ToInt32(_client.CurrentRoom.maxPlayers));
        _interface.Enter();
    }

    public override void Exit()
    {
        _client.PlayerRoomParticipationChange -= _interface.RefreshPlayerList;
        _client.PlayerReadyStatusChange -= _interface.RefreshPlayerList;
        _controller.RefreshSuccessEvent -= _interface.UpdatePlayerList;
        _controller.ReadySuccessEvent -= _interface.OnReadySucceeded;
        _interface.Exit();
    }

    public override void NextState()
    {
        throw new System.NotImplementedException();
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
    }
}


