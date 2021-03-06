﻿using System;

using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Extensions;

using Object = UnityEngine.Object;
using PlayGen.Unity.Utilities.Localization;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.ScenarioList
{
	public class ScenarioListStateInput : TickStateInput
	{
		private readonly ScenarioController _scenarioController;
		private readonly ITAlertPhotonClient _photonClient;

		private GameObject _scenarioPanel;
		private GameObject _scenarioListObject;
		private GameObject _scenarioItemPrefab;
		private Button _backButton;

		private GameObject _scenarioSelectPanel;
		private Text _selectedName;
		private Text _selectedDescription;
		private Button _selectButton;

		private bool _bestFitTick;

		public event Action BackClickedEvent;

		public ScenarioListStateInput(ITAlertPhotonClient photonClient, ScenarioController scenarioController)
		{
			_photonClient = photonClient;
			_scenarioController = scenarioController;
		}

		protected override void OnInitialize()
		{
			// Join Game Popup
			_scenarioPanel = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer");

			_backButton = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ButtonPanel/BackButtonContainer").GetComponent<Button>();

			_scenarioSelectPanel = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ScenarioSelected");
			_selectedName = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ScenarioSelected/Name Border/Name").GetComponent<Text>();
			_selectedDescription = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ScenarioSelected/Description Border/Description").GetComponent<Text>();
			_selectButton = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ButtonPanel/SelectButtonContainer").GetComponent<Button>();

			_scenarioListObject = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ScenarioListContainer/Viewport/Content");
			_scenarioItemPrefab = Resources.Load("ScenarioItem") as GameObject;

		}

		private void OnBackClick()
		{
			BackClickedEvent?.Invoke();
		}

		protected override void OnEnter()
		{
			_backButton.onClick.AddListener(OnBackClick);

			_scenarioPanel.SetActive(true);
			_backButton.BestFit();
			_selectButton.BestFit();
			_selectedName.text = string.Empty;
			_selectedDescription.text = string.Empty;
			_selectButton.gameObject.SetActive(false);
			_scenarioSelectPanel.SetActive(false);
			_bestFitTick = true;
			_scenarioController.ScenarioListSuccessEvent += OnScenarioSuccess;
			_scenarioController.GetScenarioList();
		}

		protected override void OnExit()
		{
			_backButton.onClick.RemoveListener(OnBackClick);

			_scenarioController.ScenarioListSuccessEvent -= OnScenarioSuccess;
			_scenarioPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (_bestFitTick)
			{
				_backButton.BestFit();
				_selectButton.BestFit();
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

		private void SelectScenario(ScenarioInfo scenario)
		{
			scenario.LocalizationDictionary.TryGetLocalizedStringForKey(Localization.SelectedLanguage.TwoLetterISOLanguageName, scenario.Name, out var name);
			scenario.LocalizationDictionary.TryGetLocalizedStringForKey(Localization.SelectedLanguage.TwoLetterISOLanguageName, scenario.Description, out var description);
			_selectedName.text = name ?? scenario.Name;
			_selectedDescription.text = description ?? scenario.Description;
			_scenarioSelectPanel.SetActive(true);
			_selectButton.gameObject.SetActive(true);
			_selectButton.BestFit();
			_selectButton.onClick.RemoveAllListeners();
			_selectButton.onClick.AddListener(delegate { ConfirmScenario(scenario); });
			_bestFitTick = true;
		}

		private void SelectScenarioObject(Transform selected)
		{
			foreach (Transform child in _scenarioListObject.transform)
			{
				child.Find("Selected").GetComponent<Image>().enabled = selected == child;
			}
		}

		private void ConfirmScenario(ScenarioInfo scenario)
		{
			CommandQueue.AddCommand(new SelectScenarioCommand(scenario));
			PlayGen.Unity.Utilities.Loading.Loading.Start();
		}

		private void OnScenarioSuccess(ScenarioInfo[] scenarios)
		{
			foreach (Transform child in _scenarioListObject.transform)
			{
				Object.Destroy(child.gameObject);
			}
			var offset = 0.5f;
			var height = _scenarioItemPrefab.RectTransform().sizeDelta.y;
			// Populate Scenario list UI
			foreach (var scenario in scenarios)
			{
				scenario.LocalizationDictionary.TryGetLocalizedStringForKey(Localization.SelectedLanguage.TwoLetterISOLanguageName, scenario.Name, out var name);
				var gameItem = Object.Instantiate(_scenarioItemPrefab).transform;
				gameItem.Find("Name").GetComponent<Text>().text = name ?? scenario.Name;
				var players = scenario.MinPlayerCount != scenario.MaxPlayerCount 
					? $"{scenario.MinPlayerCount}-{scenario.MaxPlayerCount}"
					: scenario.MaxPlayerCount.ToString();
				gameItem.Find("Players").GetComponent<Text>().text = players;
				gameItem.Find("Selected").GetComponent<Image>().enabled = false;
				gameItem.SetParent(_scenarioListObject.transform, false);

				// set anchors
				var gameItemRect = gameItem.RectTransform();

				gameItemRect.pivot = new Vector2(0.5f, 1f);
				gameItemRect.anchorMax = Vector2.one;
				gameItemRect.anchorMin = new Vector2(0f, 1f);

				gameItemRect.offsetMin = new Vector2(0f, offset - height);
				gameItemRect.offsetMax = new Vector2(0f, offset);

				// increment the offset
				offset -= height;
				var thisScenario = scenario;
				gameItem.GetComponent<Button>().onClick.AddListener(delegate { SelectScenario(thisScenario); });
				gameItem.GetComponent<Button>().onClick.AddListener(delegate { SelectScenarioObject(gameItem); });
				if (scenario == scenarios[0])
				{
					gameItem.GetComponent<Button>().onClick.Invoke();
				}
			}
			// Set the content box to be the correct size for our elements
			_scenarioListObject.BestFit();
			_scenarioListObject.RectTransform().sizeDelta = new Vector2(0f, offset * -1f);
		}
	}
}