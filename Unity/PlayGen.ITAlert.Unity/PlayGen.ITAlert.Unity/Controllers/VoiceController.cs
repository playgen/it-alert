using System;
using GameWork.Core.Logging.Loggers;
using PlayGen.ITAlert.Photon.Messages.Game.UI;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Controllers
{
	public class VoiceController
	{
		private readonly ITAlertPhotonClient _photonClient;
		private readonly Director _director;
	    private readonly Func<bool> _getActivated;
	    private readonly Func<bool> _getDeactivated;

	    private VoiceClient VoiceClient => _photonClient?.CurrentRoom?.VoiceClient;

        public VoiceController(
            ITAlertPhotonClient photonClient,
            Director director,
            Func<bool> getActivated,
            Func<bool> getDeactivated)
		{
			_photonClient = photonClient;
			_director = director;
		    _getActivated = getActivated;
		    _getDeactivated = getDeactivated;
		}

		/// <summary>
		/// This should never be called when there is no _photonClient.CurrentRoom as 
		/// voice functionality cannot exist outside of a room.
		/// </summary>
		public void HandleVoiceInput()
		{
			try
			{
				if (VoiceClient != null && _photonClient.CurrentRoom.Players.Count > 1)
				{
					if (_getActivated())
					{
						if (!VoiceClient?.IsTransmitting ?? false)
						{
							VoiceClient.StartTransmission();
							LogProxy.Warning("Starting voice transmission");
							if (_director?.Player != null)
							{
								_photonClient?.CurrentRoom?.Messenger.SendMessage(new PlayerVoiceActivatedMessage
																					{
									PlayerId = _director.Player.Id
								});
							}
						}
					}
					if (_getDeactivated())
					{
						if ((bool)VoiceClient?.IsTransmitting)
						{
							VoiceClient.StopTransmission();
							if (_director?.Player != null)
							{
								_photonClient?.CurrentRoom?.Messenger.SendMessage(new PlayerVoiceDeactivatedMessage
																					{
									PlayerId = _director.Player.Id
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new PhotonVoiceException("Error processing voice communication", ex);
			}
		}
	}
}