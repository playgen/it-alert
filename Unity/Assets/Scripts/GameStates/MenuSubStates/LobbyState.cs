using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;
using PlayGen.Photon.Players;
using PlayGen.SUGAR.Unity;

public class LobbyState : InputTickState
{
	private readonly LobbyController _controller;
	public event Action GameStartedEvent;

	private readonly Client _client;
	private readonly VoiceController _voiceController;
	public const string StateName = "LobbyState";

	public LobbyState(LobbyStateInput input, LobbyController controller, Client client, VoiceController voiceController) : base(input)
	{
		_controller = controller;
		_voiceController = voiceController;
		_client = client;
	}
	
	protected override void OnEnter()
	{
		_client.CurrentRoom.PlayerListUpdatedEvent += UpdateThisPlayerFromSUGAR;


	}

	protected override void OnExit()
	{
		_client.CurrentRoom.PlayerListUpdatedEvent -= UpdateThisPlayerFromSUGAR;
	}

	public override string Name
	{
		get { return StateName; }
	}

	protected override void OnTick(float deltaTime)
	{
		_voiceController.HandleVoiceInput();

		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
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
