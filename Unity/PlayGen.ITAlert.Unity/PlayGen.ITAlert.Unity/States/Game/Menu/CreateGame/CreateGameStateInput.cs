﻿using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;

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

        private bool _bestFitTick;

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

			// Create Game Listener Goes Here
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
            PlayGen.Unity.Utilities.Loading.Loading.Start();
		}

		private void OnBackClick()
		{
			BackClickedEvent?.Invoke();
		}

		protected override void OnEnter()
		{
			_createGameCloseButton.onClick.AddListener(OnBackClick);
			_createGamePopupButton.onClick.AddListener(OnCreateClick);

            PlayGen.Unity.Utilities.Loading.Loading.Stop();
			_photonClient.JoinedRoomEvent += OnJoinedRoom;
			_createGamePanel.SetActive(true);
			_createGamePanel.GetComponent<CreateGamePopupBehaviour>().ResetFields(_scenarioController.Selected);
            _buttons.Buttons.BestFit();
            _bestFitTick = true;
        }

		protected override void OnExit()
		{
			_createGameCloseButton.onClick.RemoveListener(OnBackClick);
			_createGamePopupButton.onClick.RemoveListener(OnCreateClick);

			_photonClient.JoinedRoomEvent -= OnJoinedRoom;
			_createGamePanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
            if (_bestFitTick)
            {
                _buttons.Buttons.BestFit();
                _bestFitTick = false;
            }
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