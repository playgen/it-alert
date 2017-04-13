using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Feedback
{
	public class FeedbackStateInput : TickStateInput
	{
		private readonly Dictionary<string, List<string>> _playerRankings = new Dictionary<string, List<string>>();

		private readonly Dictionary<string, List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>> _rankingObjects
			= new Dictionary<string, List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>>();

		private readonly Client _photonClient;

		private GameObject _feedbackPanel;
		private GameObject _columnPrefab;
		private GameObject _entryPrefab;
		private GameObject _slotPrefab;
		private ButtonList _buttons;
		private GameObject _rankingImage;
		private GameObject _error;
		private Button _sendButton;

		private bool _bestFitDelay;

		public event Action<Dictionary<string, int[]>> PlayerRankingsCompleteEvent;
		public event Action FeedbackSendClickedEvent;

		public FeedbackStateInput(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public bool RearrangeOrder(string previousList, string newList, int position, string name, FeedbackSlotBehaviour slot,
			FeedbackDragBehaviour drag, bool flowDown = true)
		{
			if ((previousList == null && _playerRankings[newList].Contains(name)) ||
				_playerRankings[newList].IndexOf(name) == position)
			{
				return false;
			}
			if (_playerRankings[newList].Contains(name))
			{
				var index = _playerRankings[newList].IndexOf(name);
				var objKey = _rankingObjects[newList][index].Key;
				_playerRankings[newList][index] = null;
				_rankingObjects[newList][index] = new KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>(objKey, null);
			}
			if (_playerRankings[newList][position] != null)
			{
				if (flowDown)
				{
					flowDown = false;
					for (int i = position; i < _playerRankings[newList].Count; i++)
					{
						if (_playerRankings[newList][i] == null)
						{
							flowDown = true;
							break;
						}
					}
				}
				var rearrangedSlot = _rankingObjects[newList][position + (flowDown ? 1 : -1)].Key;
				var rearrangedPlayer = _rankingObjects[newList][position].Value;
				RearrangeOrder(newList, newList, position + (flowDown ? 1 : -1), _playerRankings[newList][position], rearrangedSlot,
					rearrangedPlayer, flowDown);
			}
			_playerRankings[newList][position] = name;
			_rankingObjects[newList][position] = new KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>(slot, drag);
			if (previousList == null)
			{
				var oldSlot = drag.transform.parent;
				if (!_playerRankings.All(rank => rank.Value.Contains(name)))
				{
					var playerObj = UnityEngine.Object.Instantiate(_entryPrefab, oldSlot.transform, false);
					playerObj.GetComponent<Text>().text = name;
					playerObj.GetComponent<Text>().color = drag.GetComponent<Text>().color;
					playerObj.GetComponent<Text>().fontSize = drag.GetComponent<Text>().fontSize;
					playerObj.GetComponent<FeedbackDragBehaviour>().SetInterface(this);
				}
			}
			drag.transform.SetParent(slot.transform, false);
			// TODO: temporarily disable for release
			//_buttons.GetButton("SendButtonContainer").interactable = _playerRankings.All(rank => rank.Value.Count(r => r != null) == _photonClient.CurrentRoom.Players.Count - 1);
			_error.SetActive(!_buttons.GetButton("SendButtonContainer").interactable);
			return true;
		}

		protected override void OnInitialize()
		{
			_feedbackPanel = GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/FeedbackPanel");
			_columnPrefab = Resources.Load("FeedbackColumn") as GameObject;
			_entryPrefab = Resources.Load("FeedbackEntry") as GameObject;
			_slotPrefab = Resources.Load("FeedbackSlot") as GameObject;
			_buttons = new ButtonList("FeedbackContainer/FeedbackPanelContainer/FeedbackButtons");
			_error = GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/Error");
			_rankingImage =
				GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/FeedbackPanel/RankingImage");

			_sendButton = _buttons.GetButton("SendButtonContainer");
		}

		protected override void OnEnter()
		{
			_sendButton.onClick.AddListener(OnSendClick);

			PopulateFeedback(_photonClient.CurrentRoom.Players, _photonClient.CurrentRoom.Player);
			_feedbackPanel.transform.parent.gameObject.SetActive(true);
			_buttons.Buttons.BestFit();
		}

		protected override void OnExit()
		{
			_sendButton.onClick.RemoveListener(OnSendClick);
			_feedbackPanel.transform.parent.gameObject.SetActive(false);
		}

		private void OnSendClick()
		{
			_sendButton.gameObject.SetActive(false);

			var players = _photonClient.CurrentRoom.Players;
			var playerRankedIdsBySection = new Dictionary<string, int[]>();

			foreach (var rankingKVP in _playerRankings)
			{
				playerRankedIdsBySection[rankingKVP.Key] = rankingKVP.Value
					.Join(players,
						name => name,
						player => player.Name,
						(name, player) => player.PhotonId)
					.ToArray();
			}

			PlayerRankingsCompleteEvent(playerRankedIdsBySection);
			FeedbackSendClickedEvent?.Invoke();
		}

		private void PopulateFeedback(List<Player> players, Player current)
		{
			foreach (Transform child in _feedbackPanel.transform)
			{
				if (child != _rankingImage.transform)
				{
					GameObject.Destroy(child.gameObject);
				}
			}
			_playerRankings.Clear();
			_rankingObjects.Clear();

			//To-Do: Get the list of evaluation criteria from somewhere

			players = players.Where(p => p.PhotonId != current.PhotonId).ToList();

			var playerList = UnityEngine.Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);
			var emptySlot = UnityEngine.Object.Instantiate(_slotPrefab, playerList.transform, false);
			emptySlot.GetComponent<Image>().enabled = false;
			emptySlot.GetComponent<LayoutElement>().preferredHeight *= 1.25f;

			foreach (var player in players)
			{
				var color = new Color();
				ColorUtility.TryParseHtmlString(player.Color, out color);

				var playerSlot = UnityEngine.Object.Instantiate(_slotPrefab, playerList.transform, false);
				playerSlot.GetComponent<Image>().enabled = false;

				var playerObj = UnityEngine.Object.Instantiate(_entryPrefab, playerSlot.transform, false);
				playerObj.GetComponent<Text>().text = player.Name;
				playerObj.GetComponent<Text>().color = color;
				playerObj.GetComponent<FeedbackDragBehaviour>().SetInterface(this);
			}

			for (int i = playerList.transform.childCount; i <= 6; i++)
			{
				var playerSlot = UnityEngine.Object.Instantiate(_slotPrefab, playerList.transform, false);
				playerSlot.GetComponent<Image>().enabled = false;
			}

			bool sectionFound = true;
			int sectionCount = 1;
			while (sectionFound)
			{
				if (!Localization.HasKey("FEEDBACK_LABEL_CATEGORY_" + (sectionCount)))
				{
					sectionFound = false;
					continue;
				}
				var sectionName = Localization.Get("FEEDBACK_LABEL_CATEGORY_" + (sectionCount));
				sectionCount++;
				var sectionList = UnityEngine.Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);

				var headerObj = UnityEngine.Object.Instantiate(_entryPrefab, sectionList.transform, false);
				headerObj.GetComponent<LayoutElement>().preferredHeight *= 1.25f;
				headerObj.GetComponent<Text>().text = sectionName;
				Object.Destroy(headerObj.GetComponent<FeedbackDragBehaviour>());

				_playerRankings.Add(sectionName, new List<string>());
				_rankingObjects.Add(sectionName, new List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>());

				//foreach (var player in players)
				for (int j = 0; j < 6; j++)
				{
					var playerSlot = UnityEngine.Object.Instantiate(_slotPrefab, sectionList.transform, false);
					_playerRankings[sectionName].Add(null);
					_rankingObjects[sectionName].Add(
						new KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>(playerSlot.GetComponent<FeedbackSlotBehaviour>(),
							null));
					playerSlot.GetComponent<FeedbackSlotBehaviour>().SetList(sectionName);
				}
			}
			// TODO: temporarily disable for release
			//_buttons.GetButton("SendButtonContainer").interactable = _playerRankings.All(rank => rank.Value.Count(r => r != null) == _photonClient.CurrentRoom.Players.Count - 1);
			_error.SetActive(!_buttons.GetButton("SendButtonContainer").interactable);
			if (_error.activeSelf)
			{
				SetErrorText();
			}
			_rankingImage.transform.SetAsLastSibling();
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_feedbackPanel.transform);
			_feedbackPanel.BestFit();
			_bestFitDelay = true;
		}

		private void SetErrorText()
		{
			var firstUnfilled = _playerRankings.First(rank => rank.Value.Count(r => r != null) != _photonClient.CurrentRoom.Players.Count - 1).Key;
			_error.GetComponent<Text>().text = Localization.GetAndFormat("FEEDBACK_LABEL_ERROR", false, firstUnfilled);
		}

		protected override void OnTick(float deltaTime)
		{
			if (_bestFitDelay)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_feedbackPanel.transform);
				_feedbackPanel.BestFit();
				_bestFitDelay = false;
			}
		}
	}
}