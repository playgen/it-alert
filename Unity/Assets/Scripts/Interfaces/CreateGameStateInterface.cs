using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.Network.Client;
using PlayGen.Photon.Unity;
using UnityEngine;

public class CreateGameStateInterface : StateInterface
{
	private GameObject _createGamePanel;
	private ButtonList _buttons;

	public override void Initialize()
	{
		// Create Game Popup
		_createGamePanel = GameObjectUtilities.FindGameObject("CreateGameContainer/CreatePanelContainer");
		_buttons = new ButtonList("CreateGameContainer/CreatePanelContainer/ButtonPanel");

		var createGameCloseButton = _buttons.GetButton("BackButtonContainer");
		createGameCloseButton.onClick.AddListener(OnBackClick);

		var createGamePopupButton = _buttons.GetButton("CreateButtonContainer");
		createGamePopupButton.onClick.AddListener(OnCreateClick);
		// Create Game Listener Goes Here
	}

	private void OnCreateClick()
	{
		var details = _createGamePanel.GetComponent<CreateGamePopupBehaviour>().GetGameDetails();
		EnqueueCommand(new CreateGameCommand(details.GameName, details.MaxPlayers));
	}

	private void OnBackClick()
	{
		EnqueueCommand(new PreviousStateCommand());
	}

	public override void Enter()
	{
		_createGamePanel.SetActive(true);
		_buttons.BestFit();
		_createGamePanel.GetComponent<CreateGamePopupBehaviour>().ResetFields();
	}

	public override void Exit()
	{
		_createGamePanel.SetActive(false);
	}

	public void OnJoinedRoom(ClientRoom room)
	{
		EnqueueCommand(new NextStateCommand());
	}
}
