using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;
using PlayGen.Photon.Unity;
using UnityEngine;

public class CreateGameStateInput : TickStateInput
{
	public event Action BackClickedEvent;
	public event Action JoinedRoomEvent;

	private readonly Client _client;
	private GameObject _createGamePanel;
	private ButtonList _buttons;

	public CreateGameStateInput(Client client)
	{
		_client = client;
	}

	protected override void OnInitialize()
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
		CommandQueue.AddCommand(new CreateGameCommand(details.GameName, details.MaxPlayers));
	}

	private void OnBackClick()
	{
		BackClickedEvent();
	}

	protected override void OnEnter()
	{
		_client.JoinedRoomEvent += OnJoinedRoom;
		_createGamePanel.SetActive(true);
		_buttons.BestFit();
		_createGamePanel.GetComponent<CreateGamePopupBehaviour>().ResetFields();
	}

	protected override void OnExit()
	{
		_client.JoinedRoomEvent -= OnJoinedRoom;
		_createGamePanel.SetActive(false);
	}

	private void OnJoinedRoom(ClientRoom room)
	{
		JoinedRoomEvent();
	}
}
