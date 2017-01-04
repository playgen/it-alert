using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;

namespace PlayGen.Photon.Plugin.Extensions
{
    public static class PluginBaseExtensions
    {
        public static void BroadcastAll(this PluginBase plugin, byte eventCode, object content = null)
        {
            plugin.PluginHost.BroadcastEvent(ReciverGroup.All,
                0,
                0,
                eventCode,
                new Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.Data, content},
                    {(byte)ParameterKey.ActorNr, RoomControllerPlugin.ServerPlayerId},
                },
                0);
        }

        public static void BroadcastSpecific(this PluginBase plugin, IList<int> recieverIds, byte eventCode, object content = null)
        {
            plugin.PluginHost.BroadcastEvent(
                recieverIds,
                RoomControllerPlugin.ServerPlayerId, 
                eventCode, 
                new Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.Data, content},
                    {(byte)ParameterKey.ActorNr, RoomControllerPlugin.ServerPlayerId},
                },
                0);
        }

        public static void SetRoomOpen(this PluginBase plugin, bool isOpen)
        {
            plugin.PluginHost.SetProperties(0,
                new Hashtable()
                {
                    { GamePropertyKey.IsOpen, isOpen }
                },
                null,
                false);
        }
    }
}
