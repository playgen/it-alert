﻿using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Players;
using PlayGen.Photon.Unity.Client;
using PlayGen.Photon.Unity.Client.Voice;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.States.Game.Room
{
	public class RoomStateInput : TickStateInput
	{
		private class PlayerVoiceItem
		{
			public GameObject GameObject { get; set; }

			public Image VoiceIcon { get; set; }

			public Image PlayerGlyph { get; set; }

			public Text NameText { get; set; }
		}

		private readonly Dictionary<int, PlayerVoiceItem> _playerVoiceItems;

		private readonly Client _photonClient;
		private GameObject _chatPanel;
		private GameObject _playerChatItemPrefab;

		public Director Director { get; private set; }

		public RoomStateInput(Client photonClient)
		{
			_photonClient = photonClient;
			Director = GameObjectUtilities.FindGameObject("Game").GetComponent<Director>();
			_playerVoiceItems = new Dictionary<int, PlayerVoiceItem>();
		}

		protected override void OnInitialize()
		{
			_chatPanel = GameObjectUtilities.FindGameObject("Voice/VoicePanelContainer").gameObject;
			_playerChatItemPrefab = Resources.Load("PlayerChatEntry") as GameObject;
		}

		protected override void OnEnter()
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += PlayersUpdated;

			foreach (var playerVoiceItem in _playerVoiceItems.Values)
			{
				UnityEngine.Object.Destroy(playerVoiceItem.GameObject);
			}
			_playerVoiceItems.Clear();
		}

		protected override void OnExit()
		{
			_chatPanel.SetActive(false);
		}

		protected override void OnTick(float deltaTime)
		{
			if (_chatPanel.activeSelf)
			{
				UpdateChatPanel();
			}
		}

		private void PlayersUpdated(List<Player> players)
		{
			_chatPanel.SetActive(players.Count > 1);

			foreach (var player in players)
			{
				if (_playerVoiceItems.TryGetValue(player.PhotonId, out var playerVoiceItem) == false)
				{
					var playerItem = Object.Instantiate(_playerChatItemPrefab);


					var nameText = playerItem.transform.FindChild("Name").GetComponent<Text>();
					nameText.text = player.Name;

					var soundIcon = playerItem.transform.FindChild("SoundIcon").GetComponent<Image>();

					var playerGlyph = playerItem.transform.FindChild("Glyph").GetComponent<Image>();

					playerItem.transform.SetParent(_chatPanel.transform, false);

					playerVoiceItem = new PlayerVoiceItem()
					{
						GameObject = playerItem,
						VoiceIcon = soundIcon,
						PlayerGlyph = playerGlyph,
						NameText = nameText,
					};

					_playerVoiceItems.Add(player.PhotonId, playerVoiceItem);
				}
				UpdatePlayerVoiceItem(player, playerVoiceItem);
			}
		}

		private void UpdatePlayerVoiceItem(Player player, PlayerVoiceItem playerVoiceItem)
		{
			if (ColorUtility.TryParseHtmlString(player.Colour, out var colour))
			{
				playerVoiceItem.NameText.color = colour;
				playerVoiceItem.VoiceIcon.color = colour;
				playerVoiceItem.PlayerGlyph.color = colour;
			}
			playerVoiceItem.PlayerGlyph.sprite = Resources.Load<Sprite>($"playerglyph_{player.Glyph}");
		}

		private void UpdateChatPanel()
		{
			foreach (var status in VoiceClient.TransmittingStatuses)
			{
				if (_playerVoiceItems.TryGetValue(status.Key, out var playerVoiceItem))
				{
					playerVoiceItem.VoiceIcon.enabled = status.Value;
				}
			}
		}
	}
}