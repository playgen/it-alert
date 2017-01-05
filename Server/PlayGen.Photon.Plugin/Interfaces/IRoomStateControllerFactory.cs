using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.SUGAR;

namespace PlayGen.Photon.Plugin.Interfaces
{
	public interface IRoomStateControllerFactory
	{
		RoomStateController Create(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController);
	}
}