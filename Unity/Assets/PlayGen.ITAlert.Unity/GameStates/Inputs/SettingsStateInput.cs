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

		public event Action BackClickedEvent;

		protected override void OnInitialize()
		{
			_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
			_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
			_creator.Wipe();
			_creator.SetLabelAlignment(TextAnchor.MiddleLeft);
			_creator.Custom<Text>("Display", SettingObjectType.Label, false);
			var resolution = _creator.Resolution(960, 540);
			var fullScreen = _creator.FullScreen();
			_creator.Custom<Text>("Voice", SettingObjectType.Label, true);
			var microphone = _creator.Volume("Microphone Volume");
			var receive = _creator.Volume("Receive Volume");
			_creator.AddToLayout(_creator.HorizontalLayout("Test Microphone", 8),
				_creator.Custom<Button>("Test Microphone", SettingObjectType.Button, true));
			_creator.Custom<Text>("Sound", SettingObjectType.Label, true);
			var music = _creator.Volume("Music Volume");
			var sfx = _creator.Volume("SFX Volume");
			var buttonLayout = _creator.HorizontalLayout("Buttons");
			var cancel = _creator.Custom<Button>("Cancel", SettingObjectType.Button, true);
			_creator.AddToLayout(buttonLayout, cancel);
			cancel.onClick.AddListener(OnBackClick);
			var apply = _creator.Custom<Button>("Apply", SettingObjectType.Button, true);
			apply.onClick.AddListener(delegate { OnApplyClick(resolution, fullScreen, microphone, receive, music, sfx); });
			_creator.AddToLayout(buttonLayout, apply);
		}

		private void OnBackClick()
		{
			BackClickedEvent();
		}

		private void OnApplyClick(Dropdown resolution, Toggle fullScreen, Slider microphone, Slider receive, Slider music,
			Slider sfx)
		{
			var newResolutionSplit = resolution.options[resolution.value].text.Split('x');
			var newResolution = new Resolution
			{
				width = int.Parse(newResolutionSplit[0]),
				height = int.Parse(newResolutionSplit[1])
			};

			Screen.SetResolution(newResolution.width, newResolution.height, fullScreen.isOn);
			PlayerPrefs.SetFloat(microphone.name, microphone.value);
			PlayerPrefs.SetFloat(receive.name, receive.value);
			PlayerPrefs.SetFloat(music.name, music.value);
			PlayerPrefs.SetFloat(sfx.name, sfx.value);
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