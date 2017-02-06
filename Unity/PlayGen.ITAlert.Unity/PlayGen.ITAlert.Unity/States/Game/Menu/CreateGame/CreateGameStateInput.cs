using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.CreateGame
{
	public class CreateGameStateInput : TickStateInput
	{
		public event Action BackClickedEvent;
		public event Action JoinedRoomEvent;

		private readonly Client _photonClient;
		private readonly ScenarioController _scenarioController;

		private GameObject _createGamePanel;
		private ButtonList _buttons;
		private Button _createGamePopupButton;
		private Button _createGameCloseButton;

		public CreateGameStateInput(Client photonClient, ScenarioController scenarioController)
		{
			_photonClient = photonClient;
			_scenarioController = scenarioController;
		}

		protected override void OnInitialize()
		{
			// Create Game Popup
			_createGamePanel = GameObjectUtilities.FindGameObject("CreateGameContainer/CreatePanelContainer");
			_buttons = new ButtonList("CreateGameContainer/CreatePanelContainer/ButtonPanel");

			_createGameCloseButton = _buttons.GetButton("BackButtonContainer");
			_createGamePopupButton = _buttons.GetButton("CreateButtonContainer");

			_createGameCloseButton.onClick.AddListener(OnBackClick);
			_createGamePopupButton.onClick.AddListener(OnCreateClick);
			// Create Game Listener Goes Here
		}

		protected override void OnTerminate()
		{
			_createGameCloseButton.onClick.RemoveListener(OnBackClick);
			_createGamePopupButton.onClick.RemoveListener(OnCreateClick);
		}

		private void OnCreateClick()
		{
			var details = _createGamePanel.GetComponent<CreateGamePopupBehaviour>().GetGameDetails();
			CommandQueue.AddCommand(new CreateGameCommand(new CreateRoomSettings
			{
				Name = details.GameName,
				MinPlayers = _scenarioController.Selected.MinPlayerCount,
				MaxPlayers = details.MaxPlayers,
				CloseOnStarted = true,
				OpenOnEnded = true,
				GameScenario = _scenarioController.Selected.Name
			}));
			LoadingUtility.ShowSpinner();
		}

		private void OnBackClick()
		{
			BackClickedEvent?.Invoke();
		}

		protected override void OnEnter()
		{
			LoadingUtility.HideSpinner();
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
			_createGamePanel.SetActive(true);
			_buttons.BestFit();
			_createGamePanel.GetComponent<CreateGamePopupBehaviour>().ResetFields(_scenarioController.Selected);
		}

		protected override void OnExit()
		{
			_photonClient.JoinedRoomEvent -= OnJoinedRoom;
			_createGamePanel.SetActive(false);
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

		private void OnJoinedRoom(ClientRoom room)
		{
			JoinedRoomEvent?.Invoke();
		}
	}
}