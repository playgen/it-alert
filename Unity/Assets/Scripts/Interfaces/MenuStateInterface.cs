using GameWork.Core.States.Commands;
using GameWork.Legacy.Core.Interfacing;
using PlayGen.Photon.Unity;

using UnityEngine;

public class MenuStateInterface : TickableStateInterface
{
	private GameObject _mainMenuPanel;
	private GameObject _createGamePopup;
	private ButtonList _buttons;

	public override void Initialize()
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
		EnqueueCommand(new ChangeStateCommand(GamesListState.StateName));
	}

	private void OnCreateGameClick()
	{
		EnqueueCommand(new ChangeStateCommand(CreateGameState.StateName));
	}

	private void OnQuickMatchClick()
	{
		EnqueueCommand(new QuickGameCommand());
	}

	private void OnSettingsClick()
	{
		EnqueueCommand(new ChangeStateCommand(SettingsState.StateName));
	}

	private void OnQuitClick()
	{
		Application.Quit();
	}

	public override void Enter()
	{
		_mainMenuPanel.SetActive(true);
		_buttons.BestFit();
	}

	public override void Exit()
	{
		_mainMenuPanel.SetActive(false);
	}

	public override void Tick(float deltaTime)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnQuitClick();
		}
	}

	public void OnJoinGameSuccess(ClientRoom clientRoom)
	{
		// todo refactor states
		//EnqueueCommand(new NextStateCommand());
	}
}