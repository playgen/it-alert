using System.IO;

namespace PlayGen.ITAlert.Photon.SUGAR
{
    public class ConfigurationLoader
    {
        public static Configuration Load()
        {
            var path = Path.GetDirectoryName(typeof(Configuration).Assembly.Location) + "/SUGARConfiguration.json";
            var data = File.ReadAllText(path);

            // todo find out why referencing json here causes bad ref for the client 
            //return JsonConvert.DeserializeObject<Configuration>(data);
            
            // hackey temporary solution
            var lines = File.ReadAllLines(path);

            return new Configuration()
            {
                BaseAddress = lines[0],
                GameId = int.Parse(lines[1]),
                AccountName = lines[2],
                AccountPassword = lines[3],
                AccountSource = lines[4]
            };
        }
    }
}
