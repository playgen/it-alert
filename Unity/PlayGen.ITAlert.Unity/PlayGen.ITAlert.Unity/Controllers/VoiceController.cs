using PlayGen.Photon.Unity.Client;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class VoiceController
	{
		private VoiceClient _voiceClient;
		private readonly Client _photonClient;

		public VoiceController(Client photonClient)
		{
			_photonClient = photonClient;
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
		}

		public void HandleVoiceInput()
		{
			if (_photonClient.CurrentRoom.Players.Count > 1)
			{
				if (Input.GetKey(KeyCode.Tab) && !_voiceClient.IsTransmitting)
				{
					_voiceClient.StartTransmission();
				}
				else if (!Input.GetKey(KeyCode.Tab) && _voiceClient.IsTransmitting)
				{
					_voiceClient.StopTransmission();
				}
			}
		}

		private void OnJoinedRoom(ClientRoom room)
		{
			_voiceClient = room.VoiceClient;
		}
	}
}