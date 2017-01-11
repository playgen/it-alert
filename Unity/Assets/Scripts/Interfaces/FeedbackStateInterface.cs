using System.Collections.Generic;

using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

using PlayGen.ITAlert.Photon.Players;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FeedbackStateInterface : StateInterface
{
	private GameObject _feedbackPanel;
	private GameObject _columnPrefab;
	private GameObject _entryPrefab;
	private GameObject _slotPrefab;
	private ButtonList _buttons;
	private GameObject _rankingImage;
	private GameObject _error;
	private Dictionary<string, List<string>> _playerRankings = new Dictionary<string, List<string>>();
	private Dictionary<string, List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>> _rankingObjects = new Dictionary<string, List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>>();

	public override void Initialize()
	{
		_feedbackPanel = GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/FeedbackPanel");
		_columnPrefab = Resources.Load("FeedbackColumn") as GameObject;
		_entryPrefab = Resources.Load("FeedbackEntry") as GameObject;
		_slotPrefab = Resources.Load("FeedbackSlot") as GameObject;
		_buttons = new ButtonList("FeedbackContainer/FeedbackPanelContainer/FeedbackButtons");
		_error = GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/Error");
		_rankingImage = GameObjectUtilities.FindGameObject("FeedbackContainer/FeedbackPanelContainer/FeedbackPanel/RankingImage");

		var sendButton = _buttons.GetButton("SendButtonContainer");
		sendButton.onClick.AddListener(OnSendClick);
	}

	private void OnSendClick()
	{
		//Todo: Send selected feedback to SUGAR
		EnqueueCommand(new NextStateCommand());
	}

	public override void Enter()
	{
		_feedbackPanel.transform.parent.gameObject.SetActive(true);
		_buttons.BestFit();
	}

	public override void Exit()
	{
		_feedbackPanel.transform.parent.gameObject.SetActive(false);
	}

	public void PopulateFeedback(Player[] players, PhotonPlayer current)
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

		var evaluationSections = new[] { "Cooperation", "Leadership", "Communication"};

		players = players.Where(p => p.PhotonId != current.ID).ToArray();

		var playerList = Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);
		var emptySlot = Object.Instantiate(_slotPrefab, playerList.transform, false);
		emptySlot.GetComponent<Image>().enabled = false;
		emptySlot.GetComponent<LayoutElement>().preferredHeight *= 1.25f;

		foreach (var player in players)
		{
			var color = new Color();
			ColorUtility.TryParseHtmlString("#" + player.Color, out color);

			var playerSlot = Object.Instantiate(_slotPrefab, playerList.transform, false);
			playerSlot.GetComponent<Image>().enabled = false;

			var playerObj = Object.Instantiate(_entryPrefab, playerSlot.transform, false);
			playerObj.GetComponent<Text>().text = player.Name;
			playerObj.GetComponent<Text>().color = color;
			playerObj.GetComponent<FeedbackDragBehaviour>().SetInterface(this);
		}

		foreach (var section in evaluationSections)
		{
			var sectionList = Object.Instantiate(_columnPrefab, _feedbackPanel.transform, false);

			var headerObj = Object.Instantiate(_entryPrefab, sectionList.transform, false);
			headerObj.GetComponent<LayoutElement>().preferredHeight *= 1.25f;
			headerObj.GetComponent<Text>().text = section;

			_playerRankings.Add(section, new List<string>());
			_rankingObjects.Add(section, new List<KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>>());

			foreach (var player in players)
			{
				var color = new Color();
				ColorUtility.TryParseHtmlString("#" + player.Color, out color);

				var playerSlot = Object.Instantiate(_slotPrefab, sectionList.transform, false);
				_playerRankings[section].Add(null);
				_rankingObjects[section].Add(new KeyValuePair<FeedbackSlotBehaviour, FeedbackDragBehaviour>(playerSlot.GetComponent<FeedbackSlotBehaviour>(), null));
				playerSlot.GetComponent<FeedbackSlotBehaviour>().SetList(section);
			}
		}
		_buttons.GetButton("SendButtonContainer").interactable = _playerRankings.All(rank => rank.Value.All(r => r != null));
		_error.SetActive(!_buttons.GetButton("SendButtonContainer").interactable);
		_rankingImage.transform.SetAsLastSibling();
		RebuildLayout();
	}

	public bool RearrangeOrder(string previousList, string newList, int position, string name, FeedbackSlotBehaviour slot, FeedbackDragBehaviour drag, bool flowDown = true)
	{
		if ((previousList == null && _playerRankings[newList].Contains(name)) || _playerRankings[newList].IndexOf(name) == position)
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
		_buttons.GetButton("SendButtonContainer").interactable = _playerRankings.All(rank => rank.Value.All(r => r != null));
		_error.SetActive(!_buttons.GetButton("SendButtonContainer").interactable);
		return true;
	}

	private void RebuildLayout()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_feedbackPanel.transform);

		var textObjs = _feedbackPanel.GetComponentsInChildren<Text>();
		int smallestFontSize = 0;
		foreach (var text in textObjs)
		{
			text.resizeTextForBestFit = true;
			text.resizeTextMinSize = 1;
			text.resizeTextMaxSize = 100;
			text.cachedTextGenerator.Invalidate();
			text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
			text.resizeTextForBestFit = false;
			var newSize = text.cachedTextGenerator.fontSizeUsedForBestFit;
			var newSizeRescale = text.rectTransform.rect.size.x / text.cachedTextGenerator.rectExtents.size.x;
			if (text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y < newSizeRescale)
			{
				newSizeRescale = text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y;
			}
			newSize = Mathf.FloorToInt(newSize * newSizeRescale);
			if (newSize < smallestFontSize || smallestFontSize == 0)
			{
				smallestFontSize = newSize;
			}
		}
		foreach (var text in textObjs)
		{
			text.fontSize = smallestFontSize;
		}
	}
}