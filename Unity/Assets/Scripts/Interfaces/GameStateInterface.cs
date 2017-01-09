﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameWork.Legacy.Core.Interfacing;
using PlayGen.ITAlert.Network.Client.Voice;
using PlayGen.Photon.Players;
using UnityEngine.UI;

public class GameStateInterface : TickableStateInterface
{
	private GameObject _chatPanel;
	private GameObject _playerChatItemPrefab;
	private Dictionary<int, string> _playerColors;
	private Dictionary<int, Image> _playerVoiceIcons;

	public override void Initialize()
	{
		_chatPanel = GameObjectUtilities.FindGameObject("Voice/VoicePanelContainer").gameObject;
		_playerChatItemPrefab = Resources.Load("PlayerChatEntry") as GameObject;
	}

	public override void Enter()
	{
		
	}

	public override void Exit()
	{
		_chatPanel.SetActive(false);
	}

	public void PopulateChatPanel(ICollection<Player> players)
	{
		foreach (Transform child in _chatPanel.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		_chatPanel.SetActive(true);

		var offset = 0f;
		var maximumPlayersPossible = 6f; // the maximum number of players the game currently supports - Not the max for the room
		var height = _chatPanel.GetComponent<RectTransform>().rect.height / maximumPlayersPossible;

		//sort array into list
		var playersList = players.OrderBy(player => player.PhotonId).ToList();

		_playerVoiceIcons = new Dictionary<int, Image>();

		foreach (var player in playersList)
		{
			var playerItem = Object.Instantiate(_playerChatItemPrefab).transform;

            var color = new Color();
            ColorUtility.TryParseHtmlString("#" + player.Color, out color);

            var nameText = playerItem.FindChild("Name").GetComponent<Text>();
			nameText.text = player.Name;
			nameText.color = color;

			var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
			soundIcon.color = nameText.color;
			_playerVoiceIcons[player.PhotonId] = soundIcon;

			playerItem.SetParent(_chatPanel.transform, false);

			// set anchors
			var playerItemRect = playerItem.GetComponent<RectTransform>();

			playerItemRect.localScale = Vector3.one;

			playerItemRect.pivot = new Vector2(0.5f, 1f);
			playerItemRect.anchorMax = Vector2.one;
			playerItemRect.anchorMin = new Vector2(0f, 1f);

			playerItemRect.offsetMin = new Vector2(0f, offset - height);
			playerItemRect.offsetMax = new Vector2(0f, offset);

			// increment the offset
			offset -= height;
		}
	}

	public void SetPlayerColors(ICollection<Player> players)
	{
		_playerColors = new Dictionary<int, string>();

		foreach (var player in players)
		{
			_playerColors.Add(player.PhotonId, player.Color);
		}
	}

	public void UpdateChatPanel()
	{
		foreach (var status in VoiceClient.TransmittingStatuses)
		{
			_playerVoiceIcons[status.Key].enabled = status.Value;
		}
	}
}
