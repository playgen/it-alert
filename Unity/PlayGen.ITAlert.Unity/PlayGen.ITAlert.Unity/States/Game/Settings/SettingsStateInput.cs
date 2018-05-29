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
			_language = _creator.Language(false, true, "SETTINGS_LABEL_LANGUAGE");
			_resolution = _creator.Resolution(960, 540, null, false, true, "SETTINGS_LABEL_RESOLUTION");
			_fullScreen = _creator.FullScreen(true, "SETTINGS_LABEL_FULLSCREEN");
            _cancel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/ButtonPanel/CancelButtonContainer").GetComponent<Button>();
			_apply = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/ButtonPanel/ApplyButtonContainer").GetComponent<Button>();
		}

		private void OnBackClick()
		{
			BackClickedEvent?.Invoke();
		}

		private void OnApplyClick()
		{
			var newResolutionSplit = _resolution.options[_resolution.value].text.Split('x');
			var newResolution = new Resolution
			{
				width = int.Parse(newResolutionSplit[0]),
				height = int.Parse(newResolutionSplit[1])
			};

			Localization.UpdateLanguage(Localization.Languages[_language.value]);
			Screen.SetResolution(newResolution.width, newResolution.height, _fullScreen.isOn);

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