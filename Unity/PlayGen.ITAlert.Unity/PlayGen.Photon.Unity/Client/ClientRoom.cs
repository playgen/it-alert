using System;
using System.Linq;
using PlayGen.Photon.Messaging;
using System.Collections.Generic;
using System.Threading;
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

		private bool _isDisposed;

		public event Action<PhotonPlayer> OtherPlayerJoinedEvent;
		public event Action<PhotonPlayer> OtherPlayerLeftEvent;
		public event Action<List<Player>> PlayerListUpdatedEvent;
		public event Action<Exception> ExceptionEvent;

		public Messenger Messenger { get; private set; }

		public List<Player> Players { get; private set; }

		public RoomInfo RoomInfo => _photonClientWrapper.CurrentRoom;

		public VoiceClient VoiceClient => _voiceClient;

		public PhotonPlayer[] ListCurrentRoomPlayers => _photonClientWrapper.ListCurrentRoomPlayers;

		public bool IsMasterClient => _photonClientWrapper.IsMasterClient;

		public ManualResetEvent GetPlayersWait { get; } = new ManualResetEvent(false);

		public Player Player
		{
			get
			{
				try
				{
					GetPlayersWait.WaitOne();
					return Players.Single(p => p.PhotonId == _photonClientWrapper.Player.ID);
				}
				catch (InvalidOperationException ioex)
				{
					throw new PhotonClientException($"No player with photon id {_photonClientWrapper.Player.ID}", ioex);
				}
			}
		}

		public ClientRoom(PhotonClientWrapper photonClientWrapper, IMessageSerializationHandler messageSerializationHandler)
		{
			_photonClientWrapper = photonClientWrapper;

			_photonClientWrapper.OtherPlayerJoinedRoomEvent += OnOtherPlayerJoined;
			_photonClientWrapper.OtherPlayerLeftRoomEvent += OnOtherPlayerLeft;

			Messenger = new Messenger(messageSerializationHandler, photonClientWrapper);
			Logger.SetMessenger(Messenger);
			Logger.PlayerPhotonId = photonClientWrapper.Player.ID;
			Messenger.Subscribe((int)Channels.Players, ProcessPlayersMessage);

			_voiceClient = new VoiceClient();

			_photonClientWrapper.EventRecievedEvent += OnRecievedEvent;

			RefreshPlayers();

			Logger.LogDebug("Created ClientRoom");
		}

		~ClientRoom()
		{
			Dispose();
		}

		public void RefreshPlayers()
		{
			GetPlayersWait.Reset();

			Messenger.SendMessage(new ListPlayersMessage
			{
				PhotonId = _photonClientWrapper.Player.ID
			});
		}

		public void Leave()
		{
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
					UnityEngine.Debug.Log($"{exception}");
					ExceptionEvent(exception);
				}
			}
		}

		public void Dispose()
		{
			if (!_isDisposed)
			{
				_photonClientWrapper.OtherPlayerJoinedRoomEvent -= OnOtherPlayerJoined;
				_photonClientWrapper.OtherPlayerLeftRoomEvent -= OnOtherPlayerLeft;

				Messenger.Unsubscribe((int)Channels.Players, ProcessPlayersMessage);

				_voiceClient.Dispose();
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
				GetPlayersWait.Set();
				PlayerListUpdatedEvent?.Invoke(Players);
			}
		}

		internal void OnOtherPlayerLeft(PhotonPlayer otherPlayer)
		{
			OtherPlayerLeftEvent?.Invoke(otherPlayer);
		}

		internal void OnOtherPlayerJoined(PhotonPlayer otherPlayer)
		{
			OtherPlayerJoinedEvent?.Invoke(otherPlayer);
		}
	}
}