using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PlayGen.Photon.Unity.Client.Voice
{
	public class VoiceClient : IDisposable
	{
		private const string VoicePlayerPath = "PhotonVoicePlayer";

		private static readonly Dictionary<int, PhotonVoicePlayer> VoicePlayers = new Dictionary<int, PhotonVoicePlayer>();

		private readonly GameObject _gameObject;
		private readonly PhotonVoiceRecorder _rec;

		private bool _isDisposed;

		public bool IsEnabled => _rec?.enabled ?? false;
		public bool IsTransmitting => _rec?.IsTransmitting ?? false;

		public static Dictionary<int, bool> TransmittingStatuses
		{
			get
			{
				var isTransmitting = new Dictionary<int, bool>();

				foreach (var kvp in VoicePlayers)
				{
					if (kvp.Value.OwnerId == PhotonNetwork.player.ID)
					{
						isTransmitting[kvp.Key] = kvp.Value.IsRecording;
					}
					else
					{
						isTransmitting[kvp.Key] = kvp.Value.IsOutputting;
					}
				}

				return isTransmitting;
			}
		}

		public VoiceClient()
		{
			PhotonVoiceSettings.Instance.VoiceDetection = true;
			PhotonVoiceSettings.Instance.AutoTransmit = false;
			PhotonVoiceSettings.Instance.AutoDisconnect = true;
			PhotonVoiceSettings.Instance.AutoConnect = true;

			_gameObject = PhotonNetwork.Instantiate(VoicePlayerPath, Vector3.zero, Quaternion.identity, 0);
			_rec = _gameObject.GetComponent<PhotonVoiceRecorder>();

			_rec.enabled = true;
		}

		~VoiceClient()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_rec.enabled = false;
			Object.Destroy(_gameObject);

			_isDisposed = true;
		}

		public static void RegisterVoicePlayer(int id, PhotonVoicePlayer photonVoicePlayer)
		{
			VoicePlayers[id] = photonVoicePlayer;
		}

		public static void UnregisterVoicePlayer(int id)
		{
			VoicePlayers.Remove(id);
		}
		
		public void StartTransmission()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!PhotonNetwork.inRoom)
			{
				Log("Not in room.");
				return;
			}
			else if (_rec == null)
			{
				Log("No PhotonVoiceRecorder found. Check the PhotonVoicePrefab loaded and is configured correctly.");
				return;
			}

			_rec.Transmit = true;
		}

		public void StopTransmission()
		{
			if (!PhotonNetwork.connected)
			{
				Log("Not connected.");
				return;
			}
			else if (!PhotonNetwork.inRoom)
			{
				Log("Not in room.");
				return;
			}
			else if (_rec == null)
			{
				Log("No PhotonVoiceRecorder found. Check the PhotonVoicePrefab loaded and is configured correctly.");
				return;
			}

			_rec.Transmit = false;
		}

		private void Log(string message)
		{
			Debug.Log("Network.VoiceClient: " + message);
		}
	}
}