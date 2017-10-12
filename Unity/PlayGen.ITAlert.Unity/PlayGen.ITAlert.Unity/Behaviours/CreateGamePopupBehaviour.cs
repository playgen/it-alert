using System;

using PlayGen.ITAlert.Simulation.Configuration;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class CreateGamePopupBehaviour : MonoBehaviour
	{
		public InputField GameNameInputField;
		public InputField PlayerNumberInputField;

		public Button IncrementPlayersButton;
		public Button DecrementPlayersButton;

		private int _minPlayers = 1;
		private int _maxPlayers = 6;

		private int _playerCount = 2;

		void Awake()
		{
			PlayerNumberInputField.text = _playerCount.ToString();

			PlayerNumberInputField.onValueChanged.AddListener(OnChangePlayerCount);
			IncrementPlayersButton.onClick.AddListener(() => OnChangePlayerCount(1));
			DecrementPlayersButton.onClick.AddListener(() => OnChangePlayerCount(-1));
		}

		private void OnChangePlayerCount(int increment)
		{
			_playerCount = Math.Max(_minPlayers, Math.Min(_playerCount + increment, _maxPlayers));
			PlayerNumberInputField.text = _playerCount.ToString();
		}

		private void OnChangePlayerCount(string value)
		{
			int playerCount;

			if (Int32.TryParse(value, out playerCount))
			{
				_playerCount = Math.Max(_minPlayers, Math.Min(playerCount, _maxPlayers));
			}

			PlayerNumberInputField.text = _playerCount.ToString();
		}

		public GameDetails GetGameDetails()
		{
			return new GameDetails(GameNameInputField.text, PlayerNumberInputField.text);
		}

		public void ResetFields(ScenarioInfo scenario)
		{
			GameNameInputField.text = Guid.NewGuid().ToString().Substring(0, 8);
			PlayerNumberInputField.text = "";
			_minPlayers = scenario.MinPlayerCount;
			_maxPlayers = scenario.MaxPlayerCount;
			OnChangePlayerCount(_maxPlayers);

			// Disable editing controls if there is no range for player editing
			var isNumPlayersEditable = _minPlayers != _maxPlayers;
			SetPlayerNumberEditorsInteractable(isNumPlayersEditable);
		}

		public struct GameDetails
		{
			public string GameName;
			public int MaxPlayers;

			public GameDetails(string gameName, string maxPlayers)
			{
				GameName = gameName;
				MaxPlayers = int.Parse(maxPlayers);
			}
		}

		void Update()
		{
			if (EventSystem.current.currentSelectedGameObject == PlayerNumberInputField.gameObject)
			{
				if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
				{
					OnChangePlayerCount(1);
				}
				else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
				{
					OnChangePlayerCount(-1);
				}
			}

			if (Input.GetKeyDown(KeyCode.Tab)
				&& (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				&& EventSystem.current.currentSelectedGameObject != null)
			{
				var next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

				if (next == null)
					return;

				var inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
				{
					inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
				}
				EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
			}
			else if (Input.GetKeyDown(KeyCode.Tab) && EventSystem.current.currentSelectedGameObject != null)
			{
				var next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

				if (next == null)
					return;

				var inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
				{
					inputfield.OnPointerClick(new PointerEventData(EventSystem.current));
				}
				EventSystem.current.SetSelectedGameObject(next.gameObject, new BaseEventData(EventSystem.current));
			}
		}

		private void SetPlayerNumberEditorsInteractable(bool interactable)
		{
			IncrementPlayersButton.interactable = interactable;
			DecrementPlayersButton.interactable = interactable;
			PlayerNumberInputField.interactable = interactable;
		}
	}

}