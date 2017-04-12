using System;
using System.Linq;
using PlayGen.Photon.Messaging;
using System.Collections.Generic;
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
	public class ClientRoom : IDisposable
	{
		private readonly PhotonClientWrapper _photonClientWrapper;
		private readonly VoiceClient _voiceClient;
		private readonly ClientRoomInitializedCallback _initializedCallback;

		private bool _isDisposed;
		private bool _isInitialized;

		public Messenger Messenger { get; }
		public RoomInfo RoomInfo => _photonClientWrapper.CurrentRoom;
		public VoiceClient VoiceClient => _voiceClient;
		public PhotonPlayer[] ListCurrentRoomPlayers => _photonClientWrapper.ListCurrentRoomPlayers;
		public bool IsMasterClient => _photonClientWrapper.IsMasterClient;
		public List<Player> Players { get; private set; }
		public Player Player { get; private set; }

		public event Action<List<Player>> PlayerListUpdatedEvent;
		public event Action<Exception> ExceptionEvent;
		public delegate void ClientRoomInitializedCallback(ClientRoom clientRoom);

		public ClientRoom(PhotonClientWrapper photonClientWrapper, 
			IMessageSerializationHandler messageSerializationHandler,
			ClientRoomInitializedCallback initializedCallback)
		{
			_photonClientWrapper = photonClientWrapper;
			_initializedCallback = initializedCallback;

			Messenger = new Messenger(messageSerializationHandler, photonClientWrapper);
			Logger.SetMessenger(Messenger);
			Logger.PlayerPhotonId = photonClientWrapper.Player.ID;
			Messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);

			if (VoiceSettings.Instance.Enabled)
			{
				_voiceClient = new VoiceClient();
			}

			Players = new List<Player>();

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

		public void UpdatePlayer(Player player)
		{
			Messenger.SendMessage(new UpdatePlayerMessage
			{
				Player = player
			});
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
			var listedPlayersMessage = message as ListedPlayersMessage;
			if (listedPlayersMessage != null)
			{
				Players = listedPlayersMessage.Players;

				// Position this player as the first player in the list
				var player = Players.SingleOrDefault(p => p.PhotonId == PhotonNetwork.player.ID);

				if (player == null)
				{
					throw new PhotonClientException($"The current player with Id: {PhotonNetwork.player.ID} " +
													$"is not in the server's player list for this room.");
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