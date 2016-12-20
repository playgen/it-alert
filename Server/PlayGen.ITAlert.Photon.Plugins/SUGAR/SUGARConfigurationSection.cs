using System.Configuration;

namespace PlayGen.ITAlert.Photon.Plugins.SUGAR
{
    public class SUGARConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("BaseAddress", IsRequired = true)]
        public string BaseAddress
        {
            get { return (string)this["BaseAddress"]; }
            set { this["BaseAddress"] = value; }
        }

        [ConfigurationProperty("GameId", IsRequired = true)]
        public int GameId
        {
            get { return (int)this["GameId"]; }
            set { this["GameId"] = value; }
        }

        [ConfigurationProperty("AccountName", IsRequired = true)]
        public string AccountName
        {
            get { return (string)this["AccountName"]; }
            set { this["AccountName"] = value; }
        }

        [ConfigurationProperty("AccountPassword", IsRequired = true)]
        public string AccountPassword
        {
            get { return (string)this["AccountPassword"]; }
            set { this["AccountPassword"] = value; }
        }

        [ConfigurationProperty("AccountSource", IsRequired = true)]
        public string AccountSource
        {
            get { return (string)this["AccountSource"]; }
            set { this["AccountSource"] = value; }
        }

        public static SUGARConfigurationSection GetSection()
        {
            return ConfigurationManager.GetSection("SUGARConfiguration") as SUGARConfigurationSection;
        }        
    }
}
