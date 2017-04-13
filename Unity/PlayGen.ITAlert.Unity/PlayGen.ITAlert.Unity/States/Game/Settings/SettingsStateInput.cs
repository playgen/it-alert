using System;
using System.Linq;

using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Voice;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Settings;
using PlayGen.Unity.Utilities.Localization;

// #define VOICE_SETTINGS

namespace PlayGen.ITAlert.Unity.States.Game.Settings
{
	public class SettingsStateInput : TickStateInput
	{
		private GameObject _settingsPanel;
		private SettingCreation _creator;
		private Dropdown _language;
		private Dropdown _resolution;
		private Toggle _fullScreen;
#if VOICE_SETTINGS
		//private Toggle _voiceEnabled;
		//private Dropdown _device;
		//private Slider _receive;
#endif
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

#if VOICE_SETTINGS
			_voiceEnabled = _creator.Custom<Toggle>("SETTINGS_LABEL_VOICE_ENABLED", true);
			_voiceEnabled.onValueChanged.AddListener(OnVoiceEnabledChanged);
			_voiceEnabled.isOn = VoiceSettings.Instance.Enabled;

			_device = _creator.Custom<Dropdown>("SETTINGS_LABEL_MICROPHONE_DEVICE", false, true, true, false);
			_device.AddOptions(Microphone.devices.ToList());
			_device.interactable = _device.options.Count > 1;
			_device.onValueChanged.AddListener(OnVoiceDeviceChanged);
			_device.value = Microphone.GetPosition(VoiceSettings.Instance.RecordDevice);

			_receive = _creator.Volume("SETTINGS_LABEL_RECEIVE_VOLUME");
			_receive.onValueChanged.AddListener(OnVoicePlaybakLevelChanged);
			_receive.value = VoiceSettings.Instance.PlaybackLevel;
#endif

			var buttonLayout = _creator.HorizontalLayout("Buttons");
			_cancel = _creator.Custom<Button>("SETTINGS_BUTTON_CANCEL", true);
			_creator.AddToLayout(buttonLayout, _cancel);
			_apply = _creator.Custom<Button>("SETTINGS_BUTTON_APPLY", true);
			_creator.AddToLayout(buttonLayout, _apply);

#if VOICE_SETTINGS
			if (PlayerPrefs.HasKey("Voice Enabled"))
			{
				_voiceEnabled.isOn = PlayerPrefs.GetInt("Voice Enabled") == 1;
			}
			else
			{
				_voiceEnabled.isOn = true;
				PlayerPrefs.SetInt("Voice Enabled", _voiceEnabled.isOn ? 1 : 0);
			}
			if (PlayerPrefs.HasKey("Voice Volume"))
			{
				_receive.value = PlayerPrefs.GetFloat("Voice Volume");
			}
			else
			{
				PlayerPrefs.SetFloat("Voice Volume", _receive.value);
			}
#endif
		}

#if VOICE_SETTINGS
		private void OnVoiceEnabledChanged(bool isOn)
		{
			_receive.interactable = isOn;
			//VoiceSettings.Instance.Enabled = isOn;
		}
		private void OnVoiceDeviceChanged(int value)
		{
			//VoiceSettings.Instance.RecordDevice = Microphone.devices[value];
		}

		private void OnVoiceRecordLevelChanged(float value)
		{
			
		}

		private void OnVoicePlaybakLevelChanged(float value)
		{
			//VoiceSettings.Instance.PlaybackLevel = value;
		}
#endif
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
#if VOICE_SETTINGS
			PlayerPrefs.SetInt("Voice Enabled", _voiceEnabled.isOn ? 1 : 0);
			VoiceSettings.Instance.Enabled = _voiceEnabled.isOn;
			PlayerPrefs.SetFloat("Voice Volume", _receive.value);
#endif
			var volume = PlayerPrefs.GetInt("Voice Enabled") == 1 ? PlayerPrefs.GetFloat("Voice Volume") : 0;
			foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoicePlayer>())
			{
				voice.GetComponent<AudioSource>().volume = volume;
			}
			//if (_device.options.Count > 0)
			//{
			//	foreach (var voice in UnityEngine.Object.FindObjectsOfType<PhotonVoiceRecorder>())
			//	{
			//		voice.MicrophoneDevice = _device.options[_device.value].text;
			//	}
			//	UnityEngine.Object.FindObjectOfType<PhotonVoiceRecorder>().MicrophoneDevice = _device.options[_device.value].text;
			//}

			OnExit();
			OnEnter();
		}

		protected override void OnEnter()
		{
			_cancel.onClick.AddListener(OnBackClick);
			_apply.onClick.AddListener(OnApplyClick);
			_settingsPanel.SetActive(true);

#if VOICE_SETTINGS
			OnVoiceEnabledChanged(_voiceEnabled.isOn);

			if (_device != null)
			{
				_device.ClearOptions();
				_device.AddOptions(Microphone.devices.ToList());
				_device.interactable = _device.options.Count > 1;
			}
#endif
			_creator.RebuildLayout();
		}

		protected override void OnExit()
		{
#if VOICE_SETTINGS
			_voiceEnabled.onValueChanged.RemoveListener(OnVoiceEnabledChanged);
#endif
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