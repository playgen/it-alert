using System;

using GameWork.Core.States.Tick.Input;

using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Settings;
using PlayGen.Unity.Utilities.Localization;namespace PlayGen.ITAlert.Unity.States.Game.Settings
{
	public class SettingsStateInput : TickStateInput
	{
		private GameObject _settingsPanel;
		private SettingCreation _creator;
		private Dropdown _language;
		private Dropdown _resolution;
		private Toggle _fullScreen;
		private Button _cancel;
		private Button _apply;
		public event Action BackClickedEvent;

		protected override void OnInitialize()
		{
			_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
			_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
			_creator.Wipe();
			_creator.SetLabelAlignment(TextAnchor.MiddleLeft);
            _creator.TryLanguageForPlatform(out _language, false, true, "SETTINGS_LABEL_LANGUAGE");
            _creator.TryResolutionForPlatform(out _resolution, 960, 540, null, false, true, "SETTINGS_LABEL_RESOLUTION");
            _creator.TryFullScreenForPlatform(out _fullScreen, true, "SETTINGS_LABEL_FULLSCREEN");
            _cancel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/ButtonPanel/CancelButtonContainer").GetComponent<Button>();
			_apply = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/ButtonPanel/ApplyButtonContainer").GetComponent<Button>();
		}

		private void OnBackClick()
		{
			BackClickedEvent?.Invoke();
		}

		private void OnApplyClick()
		{
		    var fullScreen = _fullScreen != null ? _fullScreen.isOn : Screen.fullScreen;

		    if (_resolution != null)
		    {
		        var newResolutionSplit = _resolution.options[_resolution.value].text.Split('x');
		        var newResolution = new Resolution { width = int.Parse(newResolutionSplit[0]), height = int.Parse(newResolutionSplit[1]) };

		        Screen.SetResolution(newResolution.width, newResolution.height, fullScreen);
		    }

		    if (_language != null)
		    {
		        Localization.UpdateLanguage(Localization.Languages[_language.value]);
            }

		    OnExit();
			OnEnter();
		}

		protected override void OnEnter()
		{
			_cancel.onClick.AddListener(OnBackClick);
			_apply.onClick.AddListener(OnApplyClick);
			_settingsPanel.SetActive(true);
			_creator.RebuildLayout();
		}

		protected override void OnExit()
		{
			_cancel.onClick.RemoveListener(OnBackClick);
			_apply.onClick.RemoveListener(OnApplyClick);

			_settingsPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackClick();
			}
		}
	}
}