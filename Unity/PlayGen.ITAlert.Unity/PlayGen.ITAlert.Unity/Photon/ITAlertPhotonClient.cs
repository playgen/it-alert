using System;

using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Messaging.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Photon
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertPhotonClient : Client<ITAlertClientRoom, ITAlertPlayer>
	{
		public ITAlertPhotonClient(string gamePlugin, string gameVersion, IMessageSerializationHandler messageSerializationHandler) 
			: base(gamePlugin, gameVersion, messageSerializationHandler)
		{
		}

		 protected override ITAlertClientRoom CreateClientRoom(PhotonClientWrapper photonClientWrapper, 
			IMessageSerializationHandler messageSerializationHandler, 
			Action<ClientRoom<ITAlertPlayer>> initializedCallback)
		{
			return new ITAlertClientRoom(photonClientWrapper, messageSerializationHandler, initializedCallback);

		}
	}
}
