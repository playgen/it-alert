using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Settings;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.GameStates.Input
{
	public class SettingsStateInput : TickStateInput
	{
		private GameObject _settingsPanel;
		private SettingCreation _creator;
		private Dropdown _resolution;
		private Toggle _fullScreen;
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
			_creator.Custom<Text>("Display", SettingObjectType.Label, false);
			_resolution = _creator.Resolution(960, 540);
			_fullScreen = _creator.FullScreen();
			_creator.Custom<Text>("Voice", SettingObjectType.Label, true);
			_microphone = _creator.Volume("Microphone Volume");
			_receive = _creator.Volume("Receive Volume");
			_creator.AddToLayout(_creator.HorizontalLayout("Test Microphone", 8),
				_creator.Custom<Button>("Test Microphone", SettingObjectType.Button, true));
			_creator.Custom<Text>("Sound", SettingObjectType.Label, true);
			_music = _creator.Volume("Music Volume");
			_sfx = _creator.Volume("SFX Volume");
			var buttonLayout = _creator.HorizontalLayout("Buttons");
			_cancel = _creator.Custom<Button>("Cancel", SettingObjectType.Button, true);
			_creator.AddToLayout(buttonLayout, _cancel);
			_apply = _creator.Custom<Button>("Apply", SettingObjectType.Button, true);
			_creator.AddToLayout(buttonLayout, _apply);

			_cancel.onClick.AddListener(OnBackClick);
			_apply.onClick.AddListener(OnApplyClick);
		}

		protected override void OnTerminate()
		{
			_cancel.onClick.RemoveListener(OnBackClick);
			_apply.onClick.RemoveListener(OnApplyClick);
		}

		private void OnBackClick()
		{
			BackClickedEvent();
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
			PlayerPrefs.SetFloat(_microphone.name, _microphone.value);
			PlayerPrefs.SetFloat(_receive.name, _receive.value);
			PlayerPrefs.SetFloat(_music.name, _music.value);
			PlayerPrefs.SetFloat(_sfx.name, _sfx.value);

			BackClickedEvent();
		}

		protected override void OnEnter()
		{
			_settingsPanel.SetActive(true);
			_creator.RebuildLayout();
		}

		protected override void OnExit()
		{
			_settingsPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackClick();
			}
		}
	}
}