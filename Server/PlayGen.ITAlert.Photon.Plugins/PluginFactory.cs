using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace PlayGen.ITAlert.Photon.Plugins
{
    public class PluginFactory : IPluginFactory
    {
        public IGamePlugin Create(IPluginHost gameHost, string pluginName, Dictionary<string, string> config, out string errorMsg)
        {
            IGamePlugin plugin = null;

            switch (pluginName)
            {
                case RoomControllerPlugin.PluginName:
                    plugin = new RoomControllerPlugin();
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
