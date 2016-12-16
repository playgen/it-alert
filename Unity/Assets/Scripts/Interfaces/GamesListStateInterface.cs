using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.Network.Client;

using UnityEngine.UI;

public class GamesListStateInterface : StateInterface
{
	private GameObject _joinGamePanel;
	private GameObject _gameListObject;
	private GameObject _gameItemPrefab;
	private ButtonList _buttons;

	public override void Initialize()
	{
		// Join Game Popup
		_joinGamePanel = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer");
		_buttons = new ButtonList("JoinGameContainer/JoinPanelContainer/ButtonPanel");

		var backButton = _buttons.GetButton("BackButtonContainer");
		backButton.onClick.AddListener(OnBackClick);

		var refreshButton = _buttons.GetButton("RefreshButtonContainer");
		refreshButton.onClick.AddListener(OnRefreshClick);

		_gameListObject = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer/GameListContainer/Viewport/Content");
		_gameItemPrefab = Resources.Load("GameItem") as GameObject;

	}
	
	public void OnRefreshClick()
	{
		EnqueueCommand(new RefreshGamesListCommand());
	}

	private void OnBackClick()
	{
		EnqueueCommand(new PreviousStateCommand());
	}

	public override void Enter()
	{
		_joinGamePanel.SetActive(true);
		_buttons.BestFit();
		OnRefreshClick();
	}
	public override void Exit()
	{
		_joinGamePanel.SetActive(false);
	}

	private void JoinGame(string name)
	{
		EnqueueCommand(new JoinGameCommand(name));
	}

	public void OnJoinGameSuccess(ClientRoom clientRoom)
	{
		EnqueueCommand(new NextStateCommand());
	}

	public void OnGamesListSuccess(RoomInfo[] rooms)
	{

		foreach (Transform child in _gameListObject.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		var offset = 0f;
		var height = _gameItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
		// Populate Game list UI
		foreach (var room in rooms)
		{
			var gameItem = Object.Instantiate(_gameItemPrefab).transform;
			var name = room.name;
			gameItem.FindChild("Name").GetComponent<Text>().text = name;
			gameItem.FindChild("Players").GetComponent<Text>().text = room.playerCount.ToString() + "/" + room.maxPlayers.ToString();
			gameItem.SetParent(_gameListObject.transform);

			// set anchors
			var gameItemRect = gameItem.GetComponent<RectTransform>();

			gameItemRect.pivot = new Vector2(0.5f, 1f);
			gameItemRect.anchorMax = Vector2.one;
			gameItemRect.anchorMin = new Vector2(0f, 1f);

			gameItemRect.offsetMin = new Vector2(0f, offset - height);
			gameItemRect.offsetMax = new Vector2(0f, offset);

			// increment the offset
			offset -= height;

			// add listener to each button to join specifc game 
			if (room.playerCount < room.maxPlayers)
			{
				gameItem.FindChild("Join").GetComponent<Button>().onClick.AddListener(delegate { JoinGame(name); });
			}
			else
			{
				gameItem.FindChild("Join").gameObject.SetActive(false);
			}
		}
		// Set the content box to be the correct size for our elements
		_gameListObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, offset * -1f);
	}
}
