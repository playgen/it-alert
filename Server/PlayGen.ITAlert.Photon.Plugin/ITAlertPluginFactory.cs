using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Plugin;

namespace PlayGen.ITAlert.Photon.Plugin
{
    public class ITAlertPluginFactory : IPluginFactory
    {
        public IGamePlugin Create(IPluginHost gameHost, string pluginName, Dictionary<string, string> config, out string errorMsg)
        {
            IGamePlugin plugin;

            switch (pluginName)
            {
                case RoomControllerPlugin.PluginName:
                    plugin = new RoomControllerPlugin(new ITAlertMessageSerializationHandler(),
                        new ITAlertRoomStateControllerFactory());
                    break;

                default:
                    plugin = new PluginBase();
                    break;
            }

            return plugin.SetupInstance(gameHost, config, out errorMsg)
                ? plugin
                : null;
        }
    }
}
