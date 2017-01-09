using GameWork.Core.States.Commands;
using GameWork.Core.States.Tick.Input;
using PlayGen.Photon.Unity;

using UnityEngine;

public class MenuStateInput : TickStateInput
{
	private GameObject _mainMenuPanel;
	private GameObject _createGamePopup;
	private ButtonList _buttons;

	protected override void OnInitialize()
	{
		// Main Menu
		_mainMenuPanel = GameObject.Find("MainMenuContainer").transform.GetChild(0).gameObject;
		_buttons = new ButtonList("MainMenuContainer/MenuPanelContainer/MenuContainer");

		var quitButton = _buttons.GetButton("QuitButtonContainer");
		quitButton.onClick.AddListener(OnQuitClick);

		var createGameButton = _buttons.GetButton("CreateGameButtonContainer");
		createGameButton.onClick.AddListener(OnCreateGameClick);

		var joinGameButton = _buttons.GetButton("JoinGameButtonContainer");
		joinGameButton.onClick.AddListener(OnJoinGameClick);

		var quickMatchButton = _buttons.GetButton("QuickMatchButtonContainer");
		quickMatchButton.onClick.AddListener(OnQuickMatchClick);

		var settingsButton = _buttons.GetButton("SettingsButtonContainer");
		settingsButton.onClick.AddListener(OnSettingsClick);
	}

	private void OnJoinGameClick()
	{
		CommandQueue.AddCommand(new ChangeStateCommand(GamesListState.StateName));
	}

	private void OnCreateGameClick()
	{
		CommandQueue.AddCommand(new ChangeStateCommand(CreateGameState.StateName));
	}

	private void OnQuickMatchClick()
	{
		CommandQueue.AddCommand(new QuickGameCommand());
	}

	private void OnSettingsClick()
	{
		CommandQueue.AddCommand(new ChangeStateCommand(SettingsState.StateName));
	}

	private void OnQuitClick()
	{
		Application.Quit();
	}

	protected override void OnEnter()
	{
		_mainMenuPanel.SetActive(true);
		_buttons.BestFit();
	}

	protected override void OnExit()
	{
		_mainMenuPanel.SetActive(false);
	}

	protected override void OnTick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnQuitClick();
		}
	}

	public void OnJoinGameSuccess(ClientRoom clientRoom)
	{
		// todo refactor state switch
		//CommandQueue.AddCommand(new NextStateCommand());
	}
}