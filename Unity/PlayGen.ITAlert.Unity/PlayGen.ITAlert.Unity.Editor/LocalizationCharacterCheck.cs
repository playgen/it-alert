using System;
using System.Collections.Generic;

using PlayGen.Unity.Utilities.Localization;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

using PlayGen.ITAlert.Unity.Simulation;

public class LocalizationCharacterCheck : EditorWindow
{
	private Text _text;

	[MenuItem("Tools/Localization Check")]
	public static void ShowWindow()
	{
		GetWindow(typeof(LocalizationCharacterCheck), true, "Localization Check", true);
	}

	void OnGUI()
	{
		_text = (Text)EditorGUILayout.ObjectField(_text, typeof(Text), true);

		if (_text)
		{
			if (GUILayout.Button("Set", GUILayout.ExpandWidth(false)))
			{
				Localization.UpdateLanguage(Localization.SelectedLanguage != null ? Localization.SelectedLanguage.Name : "en");
				var resourceList = new[] { "Localization" };
				var languageString = string.Empty;
				foreach (var resource in resourceList)
				{
					var jsonTextAssets = Resources.LoadAll(resource, typeof(TextAsset)).Cast<TextAsset>().ToArray();
					foreach (var textAsset in jsonTextAssets)
					{
						var json = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(textAsset.text);
						var comparer = StringComparer.OrdinalIgnoreCase;
						json = json.Select(d => new Dictionary<string, string>(d, comparer)).ToList();
						foreach (var dict in json)
						{
							if (dict.TryGetValue("key", out var key))
							{
								foreach (var l in Localization.Languages)
								{
									if (dict.TryGetValue(l.Name, out var str))
									{
										foreach (var c in str)
										{
											languageString += c;
										}
									}
								}
							}
						}
					}
				}
				_text.text = languageString;
				Debug.Log(languageString);
			}
		}
	}
}
