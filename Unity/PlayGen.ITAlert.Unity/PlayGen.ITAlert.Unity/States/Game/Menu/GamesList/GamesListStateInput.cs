using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.GamesList
{
	public class GamesListStateInput : TickStateInput
	{
		private readonly GamesListController _gameListController;
		private readonly Client _photonClient;

		private GameObject _joinGamePanel;
		private GameObject _gameListObject;
		private GameObject _gameItemPrefab;
		private ButtonList _buttons;
		private Button _backButton;
		private Button _refreshButton;

		public event Action JoinGameSuccessEvent;
		public event Action BackClickedEvent;

		public GamesListStateInput(Client photonClient, GamesListController gameListController)
		{
			_photonClient = photonClient;
			_gameListController = gameListController;
		}

		protected override void OnInitialize()
		{
			// Join Game Popup
			_joinGamePanel = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer");
			_buttons = new ButtonList("JoinGameContainer/JoinPanelContainer/ButtonPanel");

			_backButton = _buttons.GetButton("BackButtonContainer");
			_refreshButton = _buttons.GetButton("RefreshButtonContainer");

			_gameListObject = GameObjectUtilities.FindGameObject("JoinGameContainer/JoinPanelContainer/GameListContainer/Viewport/Content");
			_gameItemPrefab = Resources.Load("GameItem") as GameObject;

			_backButton.onClick.AddListener(OnBackClick);
			_refreshButton.onClick.AddListener(OnRefreshClick);
		}

		protected override void OnTerminate()
		{
			_backButton.onClick.RemoveListener(OnBackClick);
			_refreshButton.onClick.RemoveListener(OnRefreshClick);
		}

		private void OnRefreshClick()
		{
			CommandQueue.AddCommand(new RefreshGamesListCommand());
			LoadingUtility.ShowSpinner();
		}

		private void OnBackClick()
		{
			BackClickedEvent();
		}

		protected override void OnEnter()
		{
			_photonClient.JoinedRoomEvent += OnJoinGameSuccess;
			_gameListController.GamesListSuccessEvent += OnGamesListSuccess;

			_joinGamePanel.SetActive(true);
			_buttons.BestFit();
			OnRefreshClick();
			LoadingUtility.ShowSpinner();
		}

		protected override void OnExit()
		{
			_photonClient.JoinedRoomEvent -= OnJoinGameSuccess;
			_gameListController.GamesListSuccessEvent -= OnGamesListSuccess;
			_joinGamePanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (_photonClient.ClientState != PlayGen.Photon.Unity.Client.ClientState.Connected)
			{
				OnBackClick();
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				OnBackClick();
			}
		}

		private void JoinGame(string name)
		{
			CommandQueue.AddCommand(new JoinGameCommand(name));
			LoadingUtility.ShowSpinner();
		}

		private void OnJoinGameSuccess(ClientRoom clientRoom)
		{
			JoinGameSuccessEvent();
		}

		private void OnGamesListSuccess(RoomInfo[] rooms)
		{
			LoadingUtility.HideSpinner();
			foreach (Transform child in _gameListObject.transform)
			{
				GameObject.Destroy(child.gameObject);
			}
			var offset = 0.5f;
			var height = _gameItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
			// Populate Game list UI
			foreach (var room in rooms)
			{
				var gameItem = Object.Instantiate(_gameItemPrefab).transform;
				var name = room.name;
				gameItem.FindChild("Name").GetComponent<Text>().text = name;
				gameItem.FindChild("Players").GetComponent<Text>().text = room.playerCount.ToString() + "/" +
																		room.maxPlayers.ToString();
				gameItem.SetParent(_gameListObject.transform, false);

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
}