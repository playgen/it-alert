using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.GameStates.RoomSubStates.Input;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.GameStates.RoomSubStates
{
	public class LobbyState : InputTickState
	{
		public const string StateName = "LobbyState";

		private readonly LobbyController _controller;
		private readonly Client _photonClient;
		private readonly VoiceController _voiceController;

		public event Action GameStartedEvent;

		public override string Name
		{
			get { return StateName; }
		}

		public LobbyState(LobbyStateInput input, LobbyController controller, Client photonClient, VoiceController voiceController)
			: base(input)
		{
			input.LeaveLobbyClickedEvent += LeaveLobby;
			_controller = controller;
			_voiceController = voiceController;
			_photonClient = photonClient;
		}

		protected override void OnEnter()
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += UpdateThisPlayerFromSUGAR;
		}

		protected override void OnExit()
		{
			if (_photonClient.CurrentRoom != null)
			{
				_photonClient.CurrentRoom.PlayerListUpdatedEvent -= UpdateThisPlayerFromSUGAR;
			}
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
			}

			if (_photonClient.CurrentRoom != null && _photonClient.CurrentRoom.IsMasterClient)
			{
				if (_photonClient.CurrentRoom.Players != null &&
					_photonClient.CurrentRoom.Players.All(p => p.State == (int) PlayGen.ITAlert.Photon.Players.State.Ready))
				{
					_controller.StartGame(false);
				}
			}
		}

		private void LeaveLobby()
		{
			_photonClient.CurrentRoom.Leave();
		}

		private void UpdateThisPlayerFromSUGAR(List<Player> players)
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent -= UpdateThisPlayerFromSUGAR;

			var player = _photonClient.CurrentRoom.Player;
			player.ExternalId = SUGARManager.CurrentUser.Id;
			player.Name = SUGARManager.CurrentUser.Name;
			_photonClient.CurrentRoom.UpdatePlayer(player);
		}
	}
}