﻿using System.Collections.Generic;
using Photon.Hive.Operations;
using Photon.Hive.Plugin;
using System.Collections;

namespace PlayGen.ITAlert.Photon.Plugins.Extensions
{
    using ExitGames.Client.Photon.LoadBalancing;

    public static class PluginBaseExtensions
    {
        public static void BroadcastAll(this PluginBase plugin, int senderId, byte eventCode, object content = null)
        {
            plugin.PluginHost.BroadcastEvent(ReciverGroup.All,
                0,
                0,
                (byte)eventCode,
                new Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.Data, content},
                    {(byte)ParameterKey.ActorNr, senderId},
                },
                0);
        }

        public static void BroadcastSpecific(this PluginBase plugin, IList<int> recieverIds, int senderId, byte eventCode, object content = null)
        {
            plugin.PluginHost.BroadcastEvent(
                recieverIds, 
                senderId, 
                (byte)eventCode, 
                new Dictionary<byte, object>()
                {
                    {(byte)ParameterKey.Data, content},
                    {(byte)ParameterKey.ActorNr, senderId},
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
