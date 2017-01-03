using System;
using System.Linq;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.GameStates.GameSubStates;

using UnityEngine;

public class PausedStateInterface : TickableStateInterface
{
	private GameObject _menuPanel;
	private ButtonList _buttons;

	public override void Initialize()
	{
		// Main Menu
		_menuPanel = GameObject.Find("PauseContainer").transform.GetChild(0).gameObject;
		_buttons = new ButtonList("PauseContainer/PausePanelContainer/PauseContainer");

		var continueButton = _buttons.GetButton("ContinueButtonContainer");
		continueButton.onClick.AddListener(OnContinueClick);

		var settingsButton = _buttons.GetButton("SettingsButtonContainer");
		settingsButton.onClick.AddListener(OnSettingsClick);

		var quitButton = _buttons.GetButton("QuitButtonContainer");
		quitButton.onClick.AddListener(OnQuitClick);
	}

	private void OnContinueClick()
	{
		EnqueueCommand(new ChangeStateCommand(PlayingState.StateName));
	}

	private void OnSettingsClick()
	{
		EnqueueCommand(new ChangeStateCommand(SettingsState.StateName));
	}

	private void OnQuitClick()
	{
		EnqueueCommand(new ChangeStateCommand(MenuState.StateName));
	}

	public override void Enter()
	{
		_menuPanel.SetActive(true);
		_buttons.BestFit();
	}

	public override void Exit()
	{
		_menuPanel.SetActive(false);
	}

	public override void Tick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnContinueClick();
		}
	}
}