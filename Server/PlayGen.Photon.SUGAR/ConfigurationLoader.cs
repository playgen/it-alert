	 Newtonsoft.Json;
	 System.IO;

namespace PlayGen.Photon.SUGAR
{
	public class ConfigurationLoader
	{
		public static Configuration Load()
		{
			var path = Path.GetDirectoryName(typeof(Configuration).Assembly.Location) + "/SUGARConfiguration.json";
			var data = File.ReadAllText(path);

			return JsonConvert.DeserializeObject<Configuration>(data);
		}
	}
}
