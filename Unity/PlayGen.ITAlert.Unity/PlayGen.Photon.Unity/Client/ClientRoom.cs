using System;
using System.Linq;
using PlayGen.Photon.Messaging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Messages;
using PlayGen.Photon.Messages.Players;
using PlayGen.Photon.Unity.Client.Voice;
using PlayGen.Photon.Messaging.Interfaces;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Exceptions;
using PlayGen.Photon.Unity.Messaging;

namespace PlayGen.Photon.Unity.Client
{
	/// <summary>
	/// Can only exist within a Client
	/// </summary>
	public class ClientRoom<TPlayer> : IDisposable
		where TPlayer : Player
	{
		private readonly PhotonClientWrapper _photonClientWrapper;
		private readonly VoiceClient _voiceClient;
		private readonly Action<ClientRoom<TPlayer>> _initializedCallback;

		private bool _isDisposed;
		private bool _isInitialized;

		public Messenger Messenger { get; }
		public RoomInfo RoomInfo => _photonClientWrapper.CurrentRoom;
		public VoiceClient VoiceClient => _voiceClient;
		public PhotonPlayer[] ListCurrentRoomPlayers => _photonClientWrapper.ListCurrentRoomPlayers;
		public bool IsMasterClient => _photonClientWrapper.IsMasterClient;
		public List<TPlayer> Players { get; private set; }
		public TPlayer Player { get; private set; }

		public event Action<List<TPlayer>> PlayerListUpdatedEvent;
		public event Action<Exception> ExceptionEvent;
		public ClientRoom(PhotonClientWrapper photonClientWrapper, 
			IMessageSerializationHandler messageSerializationHandler,
			Action<ClientRoom<TPlayer>> initializedCallback)
		{
			_photonClientWrapper = photonClientWrapper;
			_initializedCallback = initializedCallback;

			Messenger = new Messenger(messageSerializationHandler, photonClientWrapper);
			Logger.SetMessenger(Messenger);
			Logger.PlayerPhotonId = photonClientWrapper.Player.ID;
			Messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);

			//if (VoiceSettings.Instance.Enabled)
			//{
				_voiceClient = new VoiceClient();
			//}

			Players = new List<TPlayer>();

			_photonClientWrapper.EventRecievedEvent += OnRecievedEvent;

			Logger.LogDebug("Created ClientRoom");
		}

		~ClientRoom()
		{
			Dispose();
		}
		
		public void Leave()
		{
			Messenger.Dispose();
			Dispose();
			_photonClientWrapper.LeaveRoom();
		}

		public void OnRecievedEvent(byte eventCode, object content, int senderId)
		{
			if (eventCode == (byte)Photon.Messaging.EventCode.Message)
			{
				try
				{
					if (!Messenger.TryProcessMessage((byte[])content))
					{
						throw new Exception("Couldn't process as message: " + content);
					}
				}
				catch(Exception exception)
				{
					UnityEngine.Debug.LogError($"{exception}");
					ExceptionEvent?.Invoke(exception);
				}
			}
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				Messenger.Unsubscribe((int)Channels.Players, ProcessPlayersMessage);

				_voiceClient?.Dispose();
				_photonClientWrapper.EventRecievedEvent -= OnRecievedEvent;

				_isDisposed = true;
			}
		}

		private void ProcessPlayersMessage(Message message)
		{
			var listedPlayersMessage = message as ListPlayersMessage<TPlayer>;
			if (listedPlayersMessage != null)
			{
				Players = listedPlayersMessage.Players;

				// Position this player as the first player in the list
				var playerId = PhotonNetwork.player.ID;

				TPlayer player = null;
				//var player = Players.SingleOrDefault(p => p.PhotonId == playerId);
				var playerIds = Players.Select(p => p.PhotonId).ToArray();

				foreach (var p in Players)
				{
					if (p.PhotonId == playerId)
					{
						player = p;
						break;
					}
				}

				if (player == null)
				{
					//return;
					var playerIdsString = playerIds.Aggregate(new StringBuilder(), (sb, pid) => sb.Append($"{pid},"),
						sb => sb.ToString());
					throw new PhotonClientException($"The current player with Id: {playerId} " + $"is not in the player list for this room: {playerIdsString}");
				}

				Players.Remove(player);
				Players.Insert(0, player);

				Player = player;

				if (!_isInitialized)
				{
					_initializedCallback(this);
					_isInitialized = true;
				}

				PlayerListUpdatedEvent?.Invoke(Players);
			}
		}
	}
}