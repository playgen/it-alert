using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.Photon.Messaging.Interfaces;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.Interfaces;

namespace PlayGen.ITAlert.Photon.Plugin
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertRoomControllerPlugin : RoomControllerPlugin<ITAlertRoomStateController, ITAlertRoomState, ITAlertPlayerManager, ITAlertPlayer>
	{
		public ITAlertRoomControllerPlugin(IMessageSerializationHandler messageSerializationHandler, 
			IRoomStateControllerFactory<ITAlertRoomStateController, ITAlertRoomState, ITAlertPlayerManager, ITAlertPlayer> roomStateControllerFactory, 
			ExceptionHandler exceptionHandler) 
			: base(messageSerializationHandler, roomStateControllerFactory, exceptionHandler)
		{
		}
	}
}
