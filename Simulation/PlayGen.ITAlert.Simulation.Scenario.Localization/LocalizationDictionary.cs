using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Scenario.Localization
{
	public class LocalizationDictionary
	{
		private readonly Dictionary<string, Dictionary<string, string>> _dictionary;

		public string DefaultLocale { get; set; } = "en-gb";

		public LocalizationDictionary(Dictionary<string, Dictionary<string, string>> dictionary)
		{
			_dictionary = dictionary;
		}

		public bool TryGetLocalizedDictionaryForLanguage(string language, out Dictionary<string, string> textDictionary)
		{
			while (_dictionary.TryGetValue(language, out textDictionary) == false)
			{
				var localeSeperator = language.LastIndexOf("-", StringComparison.Ordinal);
				if (localeSeperator == -1)
				{
					return false;
				}
				language = language.Substring(0, localeSeperator);
			}
			return true;
		}

		public bool TryGetLocalizedStringForKey(string language, string key, out string value, bool useDefault = true)
		{
			key = key.Trim();
			if (TryGetLocalizedDictionaryForLanguage(language, out var languageDictionary))
			{
				return languageDictionary.TryGetValue(key, out value);
			}
			if (useDefault && TryGetLocalizedDictionaryForLanguage(DefaultLocale, out var defaultDictionary))
			{
				return defaultDictionary.TryGetValue(key, out value);
			}
			value = null;
			return false;
		}
	}
}
