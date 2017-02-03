using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.GameStates.Menu.ScenarioList
{
	public class ScenarioListStateInput : TickStateInput
	{
		private readonly ScenarioController _scenarioController;
		private readonly Client _photonClient;

		private GameObject _scenarioPanel;
		private GameObject _scenarioListObject;
		private GameObject _scenarioItemPrefab;
		private ButtonList _buttons;
		private Button _backButton;

		public event Action BackClickedEvent;

		public ScenarioListStateInput(Client photonClient, ScenarioController scenarioController)
		{
			_photonClient = photonClient;
			_scenarioController = scenarioController;
		}

		protected override void OnInitialize()
		{
			// Join Game Popup
			_scenarioPanel = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer");
			_buttons = new ButtonList("ScenarioContainer/ScenarioContainer/ButtonPanel");

			_backButton = _buttons.GetButton("BackButtonContainer");

			_scenarioListObject = GameObjectUtilities.FindGameObject("ScenarioContainer/ScenarioContainer/ScenarioListContainer/Viewport/Content");
			_scenarioItemPrefab = Resources.Load("ScenarioItem") as GameObject;

			_backButton.onClick.AddListener(OnBackClick);
		}

		protected override void OnTerminate()
		{
			_backButton.onClick.RemoveListener(OnBackClick);
		}

		private void OnBackClick()
		{
			BackClickedEvent();
		}

		protected override void OnEnter()
		{
			_scenarioController.ScenarioListSuccessEvent += OnScenarioSuccess;
			_scenarioController.GetScenarioList();
			_scenarioPanel.SetActive(true);
			_buttons.BestFit();
		}

		protected override void OnExit()
		{
			_scenarioController.ScenarioListSuccessEvent -= OnScenarioSuccess;
			_scenarioPanel.SetActive(false);
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

		private void SelectScenario(ScenarioInfo scenario)
		{
			CommandQueue.AddCommand(new SelectScenarioCommand(scenario));
			LoadingUtility.ShowSpinner();
		}

		private void OnScenarioSuccess(ScenarioInfo[] scenarios)
		{
			foreach (Transform child in _scenarioListObject.transform)
			{
				GameObject.Destroy(child.gameObject);
			}
			var offset = 0.5f;
			var height = _scenarioItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
			// Populate Scenario list UI
			foreach (var scenario in scenarios)
			{
				var gameItem = Object.Instantiate(_scenarioItemPrefab).transform;
				gameItem.FindChild("Name").GetComponent<Text>().text = scenario.Name;
				var players = scenario.MinPlayerCount != scenario.MaxPlayerCount ? string.Format("{0}-{1}", scenario.MinPlayerCount, scenario.MaxPlayerCount) : scenario.MaxPlayerCount.ToString();
				gameItem.FindChild("Players").GetComponent<Text>().text = players;
				gameItem.FindChild("Description").GetComponent<Text>().text = scenario.Description;
				gameItem.SetParent(_scenarioListObject.transform, false);

				// set anchors
				var gameItemRect = gameItem.GetComponent<RectTransform>();

				gameItemRect.pivot = new Vector2(0.5f, 1f);
				gameItemRect.anchorMax = Vector2.one;
				gameItemRect.anchorMin = new Vector2(0f, 1f);

				gameItemRect.offsetMin = new Vector2(0f, offset - height);
				gameItemRect.offsetMax = new Vector2(0f, offset);

				// increment the offset
				offset -= height;
				var thisScenario = scenario;
				gameItem.FindChild("Select").GetComponent<Button>().onClick.AddListener(delegate { SelectScenario(thisScenario); });
			}
			// Set the content box to be the correct size for our elements
			_scenarioListObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, offset * -1f);
		}
	}
}