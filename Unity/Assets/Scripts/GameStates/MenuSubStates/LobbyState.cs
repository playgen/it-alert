using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States;
using PlayGen.ITAlert.GameStates;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Game;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.SUGAR.Unity;

public class LobbyState : TickState
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
        _client.CurrentRoom.Messenger.Subscribe((int)PlayGen.ITAlert.Photon.Messages.Channels.Game, ProcessRoomMessage);

        _controller.ReadySuccessEvent += _interface.OnReadySucceeded;
        _controller.RefreshSuccessEvent += _interface.UpdatePlayerList;

        _client.CurrentRoom.PlayerListUpdatedEvent += UpdateThisPlayerFromSUGAR;

        _client.CurrentRoom.PlayerListUpdatedEvent += _interface.OnPlayersChanged;
        _client.JoinedRoomEvent += _interface.OnJoinedRoom;
        _client.LeftRoomEvent += _interface.OnLeaveSuccess;

        _interface.SetRoomMax(Convert.ToInt32(_client.CurrentRoom.RoomInfo.maxPlayers));
        _interface.SetRoomName(_client.CurrentRoom.RoomInfo.name);
        _interface.Enter();        
    }

    public override void Exit()
    {
        _client.CurrentRoom.Messenger.Unsubscribe((int)PlayGen.ITAlert.Photon.Messages.Channels.Game, ProcessRoomMessage);

        _client.LeftRoomEvent -= _interface.OnLeaveSuccess;
        _client.JoinedRoomEvent -= _interface.OnJoinedRoom;

        _controller.RefreshSuccessEvent -= _interface.UpdatePlayerList;
        _controller.ReadySuccessEvent -= _interface.OnReadySucceeded;

        _interface.Exit();
    }

    private void ProcessRoomMessage(Message message)
    {
        var gameStartedMessage = message as GameStartedMessage;
        if (gameStartedMessage != null)
        {
			// todo refactor states
			//ChangeState(GameState.StateName);
			return;
        }

        throw new Exception("Unhandled Room Message: " + message);
    }

	// todo refactor states
	//public override void NextState()
	//{
	//    ChangeState(GameState.StateName);
	//}

	//public override void PreviousState()
	//{

	//   ChangeState(MainMenuState.StateName);
	//}

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
        
        if (_client.CurrentRoom != null && _client.CurrentRoom.IsMasterClient)
        {
            if (_client.CurrentRoom.Players != null && _client.CurrentRoom.Players.All(p => p.State == (int)PlayGen.ITAlert.Photon.Players.State.Ready))
            {
                _controller.StartGame(false);
            }
        }

        _interface.UpdateVoiceStatuses();
    }

    private void UpdateThisPlayerFromSUGAR(List<Player> players)
    {
        _client.CurrentRoom.PlayerListUpdatedEvent -= UpdateThisPlayerFromSUGAR;

        var player = _client.CurrentRoom.Player;
        player.ExternalId = SUGARManager.CurrentUser.Id;
        player.Name = SUGARManager.CurrentUser.Name;
        _client.CurrentRoom.UpdatePlayer(player);
    }
}
