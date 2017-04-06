using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Serialization;
using Newtonsoft.Json;

namespace PlayGen.ITAlert.Simulation.Scenario.Localization
{
	public class LocalizationParser
	{
		private const string KeyColumn = "Key";

		public Dictionary<string, Dictionary<string, string>> ParseLocalization(string json)
		{
			var localizationFile = ConfigurationSerializer.Deserialize<Dictionary<string, string>[]>(json);

			// language code: key: value
			var localization = new Dictionary<string, Dictionary<string, string>>();

			foreach (var row in localizationFile)
			{
				if (row.TryGetValue(KeyColumn, out var key))
				{
					foreach (var language in row.Keys.Where(k => k.Equals(KeyColumn) == false))
					{
						if (localization.TryGetValue(language, out var languageDictionary))
						{
							languageDictionary.Add(key, row[language]);
						}
					}
				}
			}

			return localization;
		}

	}
}
