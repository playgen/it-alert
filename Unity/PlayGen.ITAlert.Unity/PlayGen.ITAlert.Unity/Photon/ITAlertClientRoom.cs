using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Messaging.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Photon
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertClientRoom : ClientRoom<ITAlertPlayer>
	{
		public ITAlertClientRoom(PhotonClientWrapper photonClientWrapper, 
			IMessageSerializationHandler messageSerializationHandler,
			Action<ClientRoom<ITAlertPlayer>> initializedCallback) 
			: base(photonClientWrapper, 
				  messageSerializationHandler, 
				  initializedCallback)
		{
		}
	}
}
