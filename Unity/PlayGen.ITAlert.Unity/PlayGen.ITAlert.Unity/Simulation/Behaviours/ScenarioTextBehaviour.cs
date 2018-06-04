using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class ScenarioTextBehaviour : EntityBehaviour
	{
		private ITAlert.Simulation.Modules.Tutorial.Components.Text _textComponent;

		private GameObject _continue;

		private bool _pulse;
		private bool _pulsingDown;

		#region Initialization

		public void Awake()
		{
			_continue = transform.FindObject("Continue");
			_continue.GetComponent<Button>().onClick.AddListener(OnContinue);
			Localization.LanguageChange += LocalizationOnLanguageChange;
		}

		private new void OnDestroy()
		{
			Localization.LanguageChange -= LocalizationOnLanguageChange;
		}

		private void LocalizationOnLanguageChange()
		{
			SetText();
		}

		protected override void OnInitialize()
		{
			if (Entity.TryGetComponent(out _textComponent))
			{
				/*LogProxy.Warning(Director.SimulationRoot.Scenario.Sequence[int.Parse(_textComponent.Value.Split(new[] { "_Frame" }, StringSplitOptions.None).Last()) - 1].ExitCondition.GetType().ToString());
				LogProxy.Warning(typeof(LogicalOperationEvaluator<ITAlert.Simulation.Simulation, SimulationConfiguration>).ToString());
				var exitCondition = Director.SimulationRoot.Scenario.Sequence[int.Parse(_textComponent.Value.Split(new[] { "_Frame" }, StringSplitOptions.None).Last()) - 1].ExitCondition;
				var exitType = exitCondition.GetType();
				_pulse = exitType == typeof(WaitForTutorialContinue) || 
						(exitType == typeof(LogicalOperationEvaluator<ITAlert.Simulation.Simulation, SimulationConfiguration>) &&
						(LogicalOperationEvaluator< ITAlert.Simulation.Simulation, SimulationConfiguration>)exitCondition.);*/
				_continue.SetActive(_textComponent.ShowContinue);
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
				GetComponentInChildren<Text>().text = value;
			}
			else
			{
				LogProxy.Error($"Could not find Localized text for key {_textComponent.Value}");
				GetComponentInChildren<Text>().text = _textComponent.Value;
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
			if (_pulse)
			{
				if (_continue != null)
				{
					var continueImage = _continue.GetComponent<Image>();
					if (continueImage != null)
					{
						continueImage.color += new Color(0, 0, 0, _pulsingDown ? -Time.deltaTime : Time.deltaTime);
						if (continueImage.color.a < 0 && _pulsingDown)
						{
							_pulsingDown = false;
						}
						if (continueImage.color.a > 1 && !_pulsingDown)
						{
							_pulsingDown = true;
						}
					}
				}
			}
		}

		protected override void OnStateUpdated()
		{
		}

		#endregion
	}
}
