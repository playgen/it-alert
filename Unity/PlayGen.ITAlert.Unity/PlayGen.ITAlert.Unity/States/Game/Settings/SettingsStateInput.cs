using System;
using System.Linq;

using GameWork.Core.States.Tick.Input;

using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Voice;

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
		private Toggle _voiceEnabled;
		private Dropdown _device;
		private Slider _receive;
		private Button _cancel;
		private Button _apply;
		public event Action BackClickedEvent;

		protected override void OnInitialize()
		{
			_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
			_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
			_creator.Wipe();
			_creator.SetLabelAlignment(TextAnchor.MiddleLeft);
			_creator.Custom<Text>("SETTINGS_LABEL_DISPLAY", false);
			_language = _creator.Language(false, true, "SETTINGS_LABEL_LANGUAGE");
			_resolution = _creator.Resolution(960, 540, null, false, true, "SETTINGS_LABEL_RESOLUTION");
			_fullScreen = _creator.FullScreen(true, "SETTINGS_LABEL_FULLSCREEN");
			_creator.Custom<Text>("SETTINGS_LABEL_VOICE", true);

			_voiceEnabled = _creator.Custom<Toggle>("SETTINGS_LABEL_VOICE_ENABLED", true);

			_device = _creator.Custom<Dropdown>("SETTINGS_LABEL_MICROPHONE_DEVICE", false, true, true, false);
			_device.AddOptions(Microphone.devices.ToList());

			_receive = _creator.Volume("SETTINGS_LABEL_RECEIVE_VOLUME");
            _cancel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/ButtonPanel/CancelButtonContainer").GetComponent<Button>();
			_apply = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer/ButtonPanel/ApplyButtonContainer").GetComponent<Button>();

			_voiceEnabled.isOn = PlayerPrefs.GetInt("Voice Enabled", 1) == 1;
			VoiceSettings.Instance.Enabled = _voiceEnabled.isOn;

			if (!PlayerPrefs.HasKey("Voice Enabled"))
			{
				PlayerPrefs.SetInt("Voice Enabled", 1);
			}

			_receive.value = PlayerPrefs.GetFloat("Voice Volume", _receive.value);
			VoiceSettings.Instance.PlaybackLevel = _receive.value;

			if (!PlayerPrefs.HasKey("Voice Volume"))
			{
				PlayerPrefs.SetFloat("Voice Volume", _receive.value);
			}

			_device.interactable = _device.options.Count > 1 && _voiceEnabled.isOn;

			foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoicePlayer>())
			{
				voice.SetVolume();
			}

			if (VoiceSettings.Instance.RecordDevice != null)
			{
				if (_device.options.Count > 0 && _voiceEnabled.isOn && _device.options.Select(o => o.text).ToList().Contains(VoiceSettings.Instance.RecordDevice))
				{
					_device.value = _device.options.Select(o => o.text).ToList().IndexOf(VoiceSettings.Instance.RecordDevice);
				}
				else
				{
					_device.value = 0;
					VoiceSettings.Instance.RecordDevice = null;
				}
			}
			else
			{
				_device.value = 0;
				VoiceSettings.Instance.RecordDevice = _device.options.Count > 0 && _voiceEnabled.isOn ? _device.options[_device.value].text : null;
			}
			foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoiceRecorder>())
			{
				voice.DeviceSet(VoiceSettings.Instance.RecordDevice);
			}
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

			PlayerPrefs.SetInt("Voice Enabled", _voiceEnabled.isOn ? 1 : 0);
			VoiceSettings.Instance.Enabled = _voiceEnabled.isOn;

			PlayerPrefs.SetFloat("Voice Volume", _receive.value);
			VoiceSettings.Instance.PlaybackLevel = _receive.value;

			foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoicePlayer>())
			{
				voice.SetVolume();
			}

			VoiceSettings.Instance.RecordDevice = _device.options.Count > 0 && _voiceEnabled.isOn ? _device.options[_device.value].text : null;
			foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoiceRecorder>())
			{
				voice.DeviceSet(VoiceSettings.Instance.RecordDevice);
			}

			OnExit();
			OnEnter();
		}

		protected override void OnEnter()
		{
			_cancel.onClick.AddListener(OnBackClick);
			_apply.onClick.AddListener(OnApplyClick);
			_settingsPanel.SetActive(true);

			_receive.interactable = VoiceSettings.Instance.Enabled;

			if (_device != null)
			{
				_device.ClearOptions();
				_device.AddOptions(Microphone.devices.ToList());
				_device.interactable = _device.options.Count > 1;
				if (_device.options.Count > 0 && _voiceEnabled.isOn && _device.options.Select(o => o.text).ToList().Contains(VoiceSettings.Instance.RecordDevice))
				{
					_device.value = _device.options.Select(o => o.text).ToList().IndexOf(VoiceSettings.Instance.RecordDevice);
				}
				else
				{
					_device.value = 0;
					VoiceSettings.Instance.RecordDevice = null;
					foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoiceRecorder>())
					{
						voice.DeviceSet(VoiceSettings.Instance.RecordDevice);
					}
				}
			}
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