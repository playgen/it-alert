using System;
using PlayGen.ITAlert.Unity.Exceptions;
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
			try
			{
				if ((_photonClient.CurrentRoom?.Players?.Count ?? 0) > 1)
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
			catch (Exception ex)
			{
				throw new PhotonVoiceException("Error processing voice communciation", ex);
			}
		}

		private void OnJoinedRoom(ClientRoom room)
		{
			_voiceClient = room.VoiceClient;
		}
	}
}