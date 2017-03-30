using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Settings;

namespace PlayGen.ITAlert.Unity.States.Game.Settings
{
	public class SettingsStateInput : TickStateInput
	{
		private GameObject _settingsPanel;
		private SettingCreation _creator;
		private Dropdown _resolution;
		private Toggle _fullScreen;
		private Toggle _voiceEnabled;
		private Slider _microphone;
		private Slider _receive;
		private Slider _music;
		private Slider _sfx;
		private Button _cancel;
		private Button _apply;

		public event Action BackClickedEvent;

		protected override void OnInitialize()
		{
			_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
			_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
			_creator.Wipe();
			_creator.SetLabelAlignment(TextAnchor.MiddleLeft);
			_creator.Custom<Text>("Display", false);
			_resolution = _creator.Resolution(960, 540);
			_fullScreen = _creator.FullScreen();
			_creator.Custom<Text>("Voice", true);
			_voiceEnabled = _creator.Custom<Toggle>("Voice Enabled", true);
			_voiceEnabled.onValueChanged.AddListener(OnVoiceEnabledChanged);
			_microphone = _creator.Volume("Microphone Volume");
			_receive = _creator.Volume("Receive Volume");
			_creator.AddToLayout(_creator.HorizontalLayout("Test Microphone", 8),
			_creator.Custom<Button>("Test Microphone", true));
			_creator.Custom<Text>("Sound", true);
			_music = _creator.Volume("Music Volume");
			_sfx = _creator.Volume("SFX Volume");
			var buttonLayout = _creator.HorizontalLayout("Buttons");
			_cancel = _creator.Custom<Button>("Close", true);
			_creator.AddToLayout(buttonLayout, _cancel);
			_apply = _creator.Custom<Button>("Apply", true);
			_creator.AddToLayout(buttonLayout, _apply);

			if (PlayerPrefs.HasKey(_voiceEnabled.name))
			{
				_voiceEnabled.isOn = PlayerPrefs.GetInt(_voiceEnabled.name) == 1;
			}
			if (PlayerPrefs.HasKey(_microphone.name))
			{
				_microphone.value = PlayerPrefs.GetFloat(_microphone.name);
			}
			if (PlayerPrefs.HasKey(_receive.name))
			{
				_receive.value = PlayerPrefs.GetFloat(_receive.name);
			}
			if (PlayerPrefs.HasKey(_music.name))
			{
				_music.value = PlayerPrefs.GetFloat(_music.name);
			}
			if (PlayerPrefs.HasKey(_sfx.name))
			{
				_sfx.value = PlayerPrefs.GetFloat(_sfx.name);
			}
		}

		private void OnVoiceEnabledChanged(bool isOn)
		{
			_microphone.interactable = isOn;
			_receive.interactable = isOn;
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

			Screen.SetResolution(newResolution.width, newResolution.height, _fullScreen.isOn);
			PlayerPrefs.SetInt(_voiceEnabled.name, _voiceEnabled.isOn ? 1 : 0);
			PlayerPrefs.SetFloat(_microphone.name, _microphone.value);
			PlayerPrefs.SetFloat(_receive.name, _receive.value);
			PlayerPrefs.SetFloat(_music.name, _music.value);
			PlayerPrefs.SetFloat(_sfx.name, _sfx.value);

			OnExit();
			OnEnter();
		}

		protected override void OnEnter()
		{
			_cancel.onClick.AddListener(OnBackClick);
			_apply.onClick.AddListener(OnApplyClick);
			OnVoiceEnabledChanged(_voiceEnabled.isOn);

			_settingsPanel.SetActive(true);
			_creator.RebuildLayout();
		}

		protected override void OnExit()
		{
			_voiceEnabled.onValueChanged.RemoveListener(OnVoiceEnabledChanged);
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