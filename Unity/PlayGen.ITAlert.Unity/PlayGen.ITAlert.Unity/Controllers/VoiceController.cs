using System;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.Photon.Unity.Client;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class VoiceController
	{
		private readonly Client _photonClient;

		private VoiceClient VoiceClient => _photonClient.CurrentRoom.VoiceClient;

		public VoiceController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		/// <summary>
		/// This should never be called when there is no _photonClient.CurrentRoom as 
		/// voice functionality cannot exist outside of a room.
		/// </summary>
		public void HandleVoiceInput()
		{
			try
			{
				if (_photonClient.CurrentRoom.Players.Count > 1)
				{
					if (Input.GetKey(KeyCode.Tab))
					{
						if (!VoiceClient.IsTransmitting)
						{
							VoiceClient.StartTransmission();
						}
					}
					else if (VoiceClient.IsTransmitting)
					{
						VoiceClient.StopTransmission();
					}
				}
			}
			catch (Exception ex)
			{
				throw new PhotonVoiceException("Error processing voice communciation", ex);
			}
		}
	}
}