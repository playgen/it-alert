using System;
using GameWork.States;
using PlayGen.ITAlert.Network;

public class LobbyState : TickableSequenceState
{
    private readonly LobbyStateInterface _interface;
    private readonly LobbyController _controller;
    private ITAlertClient _client;
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
        _controller.ReadySuccessEvent += _interface.OnReadySucceeded;
        _controller.RefreshSuccessEvent += _interface.UpdatePlayerList;
        _client.PlayerReadyStatusChange += _interface.RefreshPlayerList;
        _client.PlayerRoomParticipationChange += _interface.RefreshPlayerList;
        //_readyPlayerController.ReadyFailedEvent += _interface.OnReadyFailed;
    }

    public override void Terminate()
    {
        _client.PlayerRoomParticipationChange -= _interface.RefreshPlayerList;
        _client.PlayerReadyStatusChange -= _interface.RefreshPlayerList;
        _controller.RefreshSuccessEvent -= _interface.UpdatePlayerList;
        _controller.ReadySuccessEvent -= _interface.OnReadySucceeded;
        //_readyPlayerController.ReadyFailedEvent -= _interface.OnReadyFailed;
        _interface.Terminate();
    }

    public override void Enter()
    {
        _interface.SetRoomMax(Convert.ToInt32(_client.CurrentRoom.maxPlayers));
        _interface.Enter();
    }

    public override void Exit()
    {
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
            if (command is ReadyPlayerCommand || command is RefreshPlayerListCommand)
            {
                command.Execute(_controller);
            }
            else
            {
                command.Execute(this);
            }
        }
    }
}


