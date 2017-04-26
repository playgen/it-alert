using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Serialization;

namespace PlayGen.ITAlert.Simulation.Scenario.Localization
{
	public class LocalizationHelper
	{
		private const string KeyColumn = "Key";

		public static Dictionary<string, Dictionary<string, string>> ParseLocalization(string json, string keyPrefix = null)
		{
			var localizationFile = ConfigurationSerializer.Deserialize<Dictionary<string, string>[]>(json);

			// language code: key: value
			var localization = new Dictionary<string, Dictionary<string, string>>();

			foreach (var row in localizationFile)
			{
				if (row.TryGetValue(KeyColumn, out var key) 
					&& (keyPrefix == null || key.StartsWith(keyPrefix)))
				{
					foreach (var language in row.Keys.Where(k => k.Equals(KeyColumn) == false))
					{
						if (localization.TryGetValue(language, out var languageDictionary) == false)
						{
							languageDictionary = new Dictionary<string, string>();
							localization.Add(language, languageDictionary);
						}
						languageDictionary.Add(key, row[language]);
					}
				}
			}

			return localization;
		}

		public static LocalizationDictionary GetLocalizationFromEmbeddedResource(string keyPrefix = null)
		{
			return new LocalizationDictionary(ParseLocalization(Localization.ScenarioLocalization, keyPrefix));
		}

	}
}
