﻿using System;
using System.Collections.Generic;
using System.Linq;

using GameWork.Core.States.Tick.Input;

using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Localization;

using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Feedback
{
	public class FeedbackStateInput : TickStateInput
	{
		private readonly Dictionary<string, List<string>> _playerRankings = new Dictionary<string, List<string>>();

		private readonly Dictionary<string, List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>> _rankingObjects
			= new Dictionary<string, List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>>();

		private List<ITAlertPlayer> _players;

		private readonly ITAlertPhotonClient _photonClient;

		private GameObject _feedbackPanel;
		private GameObject _columnPrefab;
		private GameObject _entryPrefab;
		private GameObject _slotPrefab;
		private ButtonList _buttons;
		private GameObject _error;
		private Button _sendButton;

		public event Action<Dictionary<int, int[]>> PlayerRankingsCompleteEvent;
		public event Action FeedbackSendClickedEvent;

		private readonly Director _director;
		private bool _panelBestFit;
		private bool RankingsComplete => _playerRankings.All(rank => rank.Value.Count(r => r != null) == _director.Players.Count - 1);

		public FeedbackStateInput(ITAlertPhotonClient photonClient, Director director)
		{
			_photonClient = photonClient;
			_director = director;
		}

		public bool RearrangeOrder(string previousList, string newList, int position, string name, FeedbackSlotBehaviour slot,
			FeedbackDragBehaviour drag, bool flowDown = true)
		{
			if (newList == null || (previousList == null && _playerRankings[newList].Contains(name)) || _playerRankings[newList].IndexOf(name) == position)
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
					for (var i = position; i < _playerRankings[newList].Count; i++)
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
				RearrangeOrder(newList, newList, position + (flowDown ? 1 : -1), _playerRankings[newList][position], rearrangedSlot, rearrangedPlayer, flowDown);
			}
			_playerRankings[newList][position] = name;
			_rankingObjects[newList][position] = new KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>(slot, drag);
			if (previousList == null)
			{
				var oldSlot = drag.transform.parent;
				if (!_playerRankings.All(rank => rank.Value.Contains(name)))
				{
					var playerObj = Object.Instantiate(_entryPrefab, oldSlot.transform, false);
					playerObj.GetComponent<Text>().text = name;
					playerObj.GetComponent<Text>().color = drag.GetComponent<Text>().color;
					playerObj.GetComponent<Text>().fontSize = drag.GetComponent<Text>().fontSize;
					playerObj.GetComponent<FeedbackDragBehaviour>().SetInterface(this);
				}
			}
			drag.transform.SetParent(slot.transform, false);
			_buttons.GetButton("SendButtonContainer").interactable = RankingsComplete;
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

			_sendButton = _buttons.GetButton("SendButtonContainer");
		}

		protected override void OnEnter()
		{
			_sendButton.onClick.AddListener(OnSendClick);

			PopulateFeedback(_director.Players, _director.Player.ExternalId);
			_feedbackPanel.transform.parent.gameObject.SetActive(true);
			_buttons.Buttons.BestFit();
			_sendButton.gameObject.SetActive(true);
		}

		protected override void OnExit()
		{
			_sendButton.onClick.RemoveListener(OnSendClick);
			_feedbackPanel.transform.parent.gameObject.SetActive(false);
		}

		private void OnSendClick()
		{
			_sendButton.gameObject.SetActive(false);

			var playerRankings = new Dictionary<int, int[]>();

			foreach (var player in _players)
			{
				var playerRanking = new int[_playerRankings.Count];
				var i = 0;
				foreach (var rankingcategory in _playerRankings)
				{
					playerRanking[i++] = rankingcategory.Value.IndexOf(player.Name);
				}
				playerRankings.Add(player.PhotonId, playerRanking);
			}

			PlayerRankingsCompleteEvent?.Invoke(playerRankings);
			FeedbackSendClickedEvent?.Invoke();
		}

		private void PopulateFeedback(List<ITAlertPlayer> players, int currentplayerPhotonId)
		{
			foreach (Transform child in _feedbackPanel.transform)
			{
				Object.Destroy(child.gameObject);
			}
			_playerRankings.Clear();
			_rankingObjects.Clear();

			//ToDo: Get the list of evaluation criteria from somewhere

			_players = players.Where(p => p.ExternalId != currentplayerPhotonId).ToList();

			var rankList = Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);
			var emptyRank = Object.Instantiate(_slotPrefab, rankList.transform, false);
			emptyRank.GetComponent<Image>().enabled = false;
			emptyRank.GetComponent<LayoutElement>().preferredHeight *= 1.25f;
			for (var i = 0; i < 6; i++)
			{ 
				var rankSlot = Object.Instantiate(_slotPrefab, rankList.transform, false);
				rankSlot.GetComponent<Image>().enabled = false;
				var rankObj = Object.Instantiate(_entryPrefab, rankSlot.transform, false);
				rankObj.GetComponent<Text>().text = (i+1).ToString();
				rankObj.GetComponent<Text>().alignment = TextAnchor.MiddleRight;
				Object.Destroy(rankObj.GetComponent<FeedbackDragBehaviour>());
			}

			var sectionFound = true;
			var sectionCount = 1;
			while (sectionFound)
			{
				if (!Localization.HasKey("FEEDBACK_LABEL_CATEGORY_" + (sectionCount)))
				{
					sectionFound = false;
					continue;
				}
				var sectionName = Localization.Get("FEEDBACK_LABEL_CATEGORY_" + (sectionCount));
				sectionCount++;
				var sectionList = Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);

				var headerObj = Object.Instantiate(_entryPrefab, sectionList.transform, false);
				headerObj.GetComponent<LayoutElement>().preferredHeight *= 1.25f;
				headerObj.GetComponent<Text>().text = sectionName;
				Object.Destroy(headerObj.GetComponent<FeedbackDragBehaviour>());

				_playerRankings.Add(sectionName, new List<string>());
				_rankingObjects.Add(sectionName, new List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>());

				//foreach (var player in players)
				for (var j = 0; j < 6; j++)
				{
					var playerSlot = Object.Instantiate(_slotPrefab, sectionList.transform, false);
					_playerRankings[sectionName].Add(null);
					_rankingObjects[sectionName].Add(new KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>(playerSlot.GetComponent<FeedbackSlotBehaviour>(), null));
					playerSlot.GetComponent<FeedbackSlotBehaviour>().SetList(sectionName);
				}
			}

			var playerList = Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);
			var emptySlot = Object.Instantiate(_slotPrefab, playerList.transform, false);
			emptySlot.GetComponent<Image>().enabled = false;
			emptySlot.GetComponent<LayoutElement>().preferredHeight *= 1.25f;

			foreach (var player in _players)
			{
				ColorUtility.TryParseHtmlString(player.Colour, out var colour);

				var playerSlot = Object.Instantiate(_slotPrefab, playerList.transform, false);
				playerSlot.GetComponent<Image>().enabled = false;

				var playerObj = Object.Instantiate(_entryPrefab, playerSlot.transform, false);
				playerObj.GetComponent<Text>().text = player.Name;
				playerObj.GetComponent<Text>().color = colour;
				playerObj.GetComponent<FeedbackDragBehaviour>().SetInterface(this);
			}

			for (var i = playerList.transform.childCount; i <= 6; i++)
			{
				var playerSlot = Object.Instantiate(_slotPrefab, playerList.transform, false);
				playerSlot.GetComponent<Image>().enabled = false;
			}

			_buttons.GetButton("SendButtonContainer").interactable = RankingsComplete;
			_error.SetActive(!_buttons.GetButton("SendButtonContainer").interactable);
			if (_error.activeSelf)
			{
				SetErrorText();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(_feedbackPanel.RectTransform());
			_panelBestFit = true;
		}

		protected override void OnTick(float deltaTime)
		{
			if (_panelBestFit)
			{
				_feedbackPanel.BestFit();
				_panelBestFit = false;
			}
		}

		private void SetErrorText()
		{
			var firstUnfilled = _playerRankings.First(rank => rank.Value.Count(r => r != null) != _director.Players.Count - 1).Key;
			_error.GetComponent<Text>().text = Localization.GetAndFormat("FEEDBACK_LABEL_ERROR", false, firstUnfilled);
		}
	}
}