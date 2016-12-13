using System.Collections.Generic;

using UnityEngine;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using UnityEngine.UI;

public class SettingsStateInterface : StateInterface
{
	private GameObject _settingsPanel;
	private SettingCreation _creator;

	public override void Initialize()
	{
		_settingsPanel = GameObjectUtilities.FindGameObject("SettingsContainer/SettingsPanelContainer");
		_creator = _settingsPanel.GetComponentInChildren<SettingCreation>();
		_creator.Wipe();
		_creator.SetLabelAlignment(TextAnchor.MiddleLeft);
		_creator.Custom<Text>("Display", SettingObjectType.Label, false);
		var resolution = _creator.Resolution(960, 540);
		var fullScreen = _creator.FullScreen();
		var quality = _creator.Quality();
		_creator.Custom<Text>("Voice", SettingObjectType.Label, true);
		var microphone = _creator.Volume("Microphone Volume");
		var receive = _creator.Volume("Receive Volume");
		_creator.AddToLayout(_creator.HorizontalLayout("Test Microphone", 8), _creator.Custom<Button>("Test Microphone", SettingObjectType.Button, true));
		_creator.Custom<Text>("Sound", SettingObjectType.Label, true);
		var music = _creator.Volume("Music Volume");
		var sfx = _creator.Volume("SFX Volume");
		var buttonLayout = _creator.HorizontalLayout("Buttons");
		var apply = _creator.Custom<Button>("Apply", SettingObjectType.Button, true);
		apply.onClick.AddListener(delegate { OnApplyClick(resolution, fullScreen, quality, microphone, receive, music, sfx); });
		_creator.AddToLayout(buttonLayout, apply);
		var cancel = _creator.Custom<Button>("Cancel", SettingObjectType.Button, true);
		_creator.AddToLayout(buttonLayout, cancel);
		cancel.onClick.AddListener(OnBackClick);
	}

	private void OnBackClick()
	{
		EnqueueCommand(new PreviousStateCommand());
	}

	private void OnApplyClick(Dropdown resolution, Toggle fullScreen, Dropdown quality, Slider microphone, Slider receive, Slider music, Slider sfx)
	{
		var newResolutionSplit = resolution.options[resolution.value].text.Split('x');
		var newResolution = new Resolution { width = int.Parse(newResolutionSplit[0]), height = int.Parse(newResolutionSplit[1]) };

		Screen.SetResolution(newResolution.width, newResolution.height, fullScreen.isOn);
		QualitySettings.SetQualityLevel(quality.value, true);
		PlayerPrefs.SetFloat(microphone.name, microphone.value);
		PlayerPrefs.SetFloat(receive.name, receive.value);
		PlayerPrefs.SetFloat(music.name, music.value);
		PlayerPrefs.SetFloat(sfx.name, sfx.value);
		_creator.RebuildLayout();
		OnBackClick();
	}

	public override void Enter()
	{
		_settingsPanel.SetActive(true);
		_creator.RebuildLayout();
	}
	public override void Exit()
	{
		_settingsPanel.SetActive(false);
	}
}
