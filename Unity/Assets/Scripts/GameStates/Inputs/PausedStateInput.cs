using GameWork.Core.States.Commands;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.GameStates.GameSubStates;
using UnityEngine;

public class PausedStateInput : TickStateInput
{
	private GameObject _menuPanel;
	private ButtonList _buttons;

	protected override void OnInitialize()
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
		CommandQueue.AddCommand(new ChangeStateCommand(PlayingState.StateName));
	}

	private void OnSettingsClick()
	{
		CommandQueue.AddCommand(new ChangeStateCommand(SettingsState.StateName));
	}

	private void OnQuitClick()
	{
		CommandQueue.AddCommand(new ChangeStateCommand(MenuState.StateName));
	}

	protected override void OnEnter()
	{
		_menuPanel.SetActive(true);
		_buttons.BestFit();
	}

	protected override void OnExit()
	{
		_menuPanel.SetActive(false);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnContinueClick();
		}
	}
}