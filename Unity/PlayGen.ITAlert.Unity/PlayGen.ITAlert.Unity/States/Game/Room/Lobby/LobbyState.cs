using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.Photon.Messages.Players;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Lobby
{
	public class LobbyState : InputTickState
	{
		public const string StateName = "LobbyState";
		public override string Name => StateName;

		private readonly LobbyController _controller;
		private readonly ITAlertPhotonClient _photonClient;

		public event Action GameStartedEvent;
		
		public LobbyState(LobbyStateInput input, LobbyController controller, ITAlertPhotonClient photonClient)
			: base(input)
		{
			input.LeaveLobbyClickedEvent += LeaveLobby;
			_controller = controller;
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
			if (CommandQueue.TryTakeFirstCommand(out var command))
			{
				if (command is LeaveRoomCommand leaveCommand)
				{
					leaveCommand.Execute(_controller);
					return;
				}

				if (command is ReadyPlayerCommand readyCommand)
				{
					readyCommand.Execute(_controller);
					return;
				}
				
				var colorCommand = command as ChangePlayerColorCommand;
			    colorCommand?.Execute(_controller);
			}
		}

		private void LeaveLobby()
		{
			_photonClient.CurrentRoom.Leave();
		}

		// ReSharper disable once InconsistentNaming
		private void UpdateThisPlayerFromSUGAR(List<ITAlertPlayer> players)
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent -= UpdateThisPlayerFromSUGAR;

			var player = _photonClient.CurrentRoom.Player;
			player.ExternalId = SUGARManager.CurrentUser.Id;
			player.Name = SUGARManager.CurrentUser.Name;
			player.RageClassId = SUGARManager.GroupId;
			SendPlayerUpdate(player);
		}

		private void SendPlayerUpdate(ITAlertPlayer player)
		{
			_photonClient.CurrentRoom.Messenger.SendMessage(new UpdatePlayerMessage<ITAlertPlayer> {
				Player = player
			});
		}

		protected virtual void OnGameStartedEvent()
		{
			GameStartedEvent?.Invoke();
		}
	}
}