using System.Collections.Generic;
using UnityEngine;

namespace PlayGen.Photon.Unity.Client.Voice
{
	public class VoiceClient
	{
		private const string VoicePlayerPath = "PhotonVoicePlayer";

		private static readonly Dictionary<int, PhotonVoicePlayer> VoicePlayers = new Dictionary<int, PhotonVoicePlayer>();

		private readonly PhotonClientWrapper _photonClientWrapper;

		private PhotonVoiceRecorder _rec;
		
		public bool IsEnabled
		{
			get { return _rec.enabled; }
		}

		public bool IsTransmitting
		{
			get { return false; //_rec.IsTransmitting; }
			}
		}

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
		}

		public static void RegisterVoicePlayer(int id, PhotonVoicePlayer photonVoicePlayer)
		{
			VoicePlayers[id] = photonVoicePlayer;
		}

		public static void UnregisterVoicePlayer(int id)
		{
			VoicePlayers.Remove(id);
		}

		public void OnJoinedRoom()
		{
			//var gameObject = PhotonNetwork.Instantiate(VoicePlayerPath, Vector3.zero, Quaternion.identity, 0);
			//_rec = gameObject.GetComponent<PhotonVoiceRecorder>();

			//_rec.enabled = true;
		}

		public void OnLeftRoom()
		{
			//_rec.enabled = false;
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

			//_rec.Transmit = true;
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
			
			//_rec.Transmit = false;
		}

		private void Log(string message)
		{
			Debug.Log("Network.VoiceClient: " + message);
		}
	}
}