using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ScenarioTextBehaviour : EntityBehaviour
	{
		private ITAlert.Simulation.Modules.Tutorial.Components.Text _textComponent;

		private Text _text;
		private GameObject _continue;
		private Button _continueButton;

		#region Initialization

		public void Awake()
		{
			_text = GetComponentInChildren<Text>();
			_continue = transform.Find("Continue").gameObject;
			_continue.GetComponent<Button>().onClick.AddListener(OnContinue);
			Localization.LanguageChange += LocalizationOnLanguageChange;
		}

		private void LocalizationOnLanguageChange()
		{
			SetText();
		}

		protected override void OnInitialize()
		{
			if (Entity.TryGetComponent(out _textComponent))
			{
				if (_textComponent.ShowContinue)
				{
					_continue.SetActive(true);
				}
				SetText();
			}
			else
			{
				throw new EntityInitializationException($"Could not load all required components for Entity Id {Entity.Id}");
			}
		}

		private void SetText()
		{
			var language = Localization.SelectedLanguage.TwoLetterISOLanguageName;
			if (Director.SimulationRoot.Scenario.LocalizationDictionary.TryGetLocalizedStringForKey(language,
				_textComponent.Value, out var value))
			{
				if (string.IsNullOrEmpty(value))
				{
					Director.SimulationRoot.Scenario.LocalizationDictionary.TryGetLocalizedStringForKey(
						Director.SimulationRoot.Scenario.LocalizationDictionary.DefaultLocale,
						_textComponent.Value, out value);
				}
				_text.text = value;
			}
			else
			{
				LogProxy.Error($"Could not find Localized text for key {_textComponent.Value}");
				_text.text = _textComponent.Value;
			}
		}

		private void OnContinue()
		{
			PlayerCommands.Continue();

		}

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
		}

		protected override void OnStateUpdated()
		{
		}

		#endregion
	}
}
