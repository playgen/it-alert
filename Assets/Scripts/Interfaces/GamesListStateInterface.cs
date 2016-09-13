using ExitGames.Client.Photon;
using UnityEngine;
using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine.UI;

public class GamesListStateInterface : StateInterface
{
	private GameObject _joinGamePanel;
	private GameObject _gameListObject;
	private GameObject _gameItemPrefab;

	public override void Initialize()
	{
		// Join Game Popup
		_joinGamePanel = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer");
		var panelButtons = new ButtonList("JoinGameContainer/JoinPanelContainer/ButtonPanel");

		var backButton = panelButtons.GetButton("BackButtonContainer");
		backButton.onClick.AddListener(OnBackClick);

		var refreshButton = panelButtons.GetButton("RefreshButtonContainer");
		refreshButton.onClick.AddListener(OnRefreshClick);

		_gameListObject = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer/GameListContainer/Viewport/Content");
		_gameItemPrefab = Resources.Load("Prefabs/GameItem") as GameObject;
	}

	private void OnRefreshClick()
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
        OnRefreshClick();
	}

	public override void Exit()
	{
		_joinGamePanel.SetActive(false);
	}

	public void OnGamesListSuccess(RoomInfo[] rooms)
	{

        foreach (Transform child in _gameListObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var offset = 0f;
		var height = 100f;
		// Populate Game list UI
		foreach (var room in rooms)
		{
			var gameItem = Object.Instantiate(_gameItemPrefab).transform;
			gameItem.FindChild("Name").GetComponent<Text>().text = room.name;
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

			// TODO: add listener to each button to join specifc game 
			//gameItem.FindChild("Join").GetComponent<Button>().onClick.AddListener(delegate{});
		}
		// Set the content box to be the correct size for our elements
		_gameListObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, offset * -1f);
	}
}
