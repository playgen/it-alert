using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Voice;
using PlayGen.Unity.Utilities.Localization;

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

		private Dictionary<int, PlayerVoiceItem> _playerVoiceItems;

		private readonly ITAlertPhotonClient _photonClient;
		private GameObject _chatPanel;
		private GameObject _playerChatItemPrefab;

		public Director Director { get; }

		public RoomStateInput(ITAlertPhotonClient photonClient)
		{
			_photonClient = photonClient;
			Director = GameObjectUtilities.FindGameObject("Game").GetComponent<Director>();
			_playerVoiceItems = new Dictionary<int, PlayerVoiceItem>();
		}

		protected override void OnInitialize()
		{
			_chatPanel = GameObjectUtilities.FindGameObject("Voice/VoicePanelContainer").gameObject;
			_playerChatItemPrefab = Resources.Load("PlayerChatEntry") as GameObject;
			Localization.LanguageChange += OnLanguageChange;
			OnLanguageChange();
		}

		protected override void OnEnter()
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += PlayersUpdated;


			foreach (var playerVoiceItem in _chatPanel.transform)
			{
				if (((Transform)playerVoiceItem).gameObject.name != "PushToTalk")
				{
					Object.Destroy(((Transform)playerVoiceItem).gameObject);
				}
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

		private void PlayersUpdated(List<ITAlertPlayer> players)
		{
			_chatPanel.SetActive(players.Count > 1);

			foreach (var player in players)
			{
				if (_playerVoiceItems.TryGetValue(player.PhotonId, out var playerVoiceItem) == false)
				{
					var playerItem = Object.Instantiate(_playerChatItemPrefab);


					var nameText = playerItem.transform.Find("Name").GetComponent<Text>();
					nameText.text = player.Name;

					var soundIcon = playerItem.transform.Find("SoundIcon").GetComponent<Image>();

					var playerGlyph = playerItem.transform.Find("Glyph").GetComponent<Image>();

					playerItem.transform.SetParent(_chatPanel.transform, false);

					playerVoiceItem = new PlayerVoiceItem
										{
						GameObject = playerItem,
						VoiceIcon = soundIcon,
						PlayerGlyph = playerGlyph,
						NameText = nameText
					};

					_playerVoiceItems.Add(player.PhotonId, playerVoiceItem);
				}
				UpdatePlayerVoiceItem(player, playerVoiceItem);
			}
			foreach (var playerVoiceItem in _playerVoiceItems)
			{
				if (players.All(p => p.PhotonId != playerVoiceItem.Key))
				{
					Object.Destroy(playerVoiceItem.Value.GameObject);
				}
			}
			_playerVoiceItems = _playerVoiceItems.Where(p => p.Value.GameObject != null).ToDictionary(p => p.Key, p => p.Value);
		}

		private void UpdatePlayerVoiceItem(ITAlertPlayer player, PlayerVoiceItem playerVoiceItem)
		{
			if (ColorUtility.TryParseHtmlString(player.Colour, out var colour))
			{
				playerVoiceItem.NameText.color = colour;
				playerVoiceItem.VoiceIcon.color = colour;
				playerVoiceItem.PlayerGlyph.color = colour;
			}
			playerVoiceItem.PlayerGlyph.sprite = Resources.Load<Sprite>($"playerglyph_{player.Glyph}");
			if (playerVoiceItem.NameText.text != player.Name)
			{
				playerVoiceItem.NameText.text = player.Name;
			}
		}

		private void UpdateChatPanel()
		{
			foreach (var status in VoiceClient.TransmittingStatuses)
			{
				if (_playerVoiceItems.TryGetValue(status.Key, out var playerVoiceItem))
				{
                    if (playerVoiceItem != null && playerVoiceItem.VoiceIcon != null)
                    {
                        playerVoiceItem.VoiceIcon.enabled = status.Value;
                    }
				}
			}
		}

		private void OnLanguageChange()
		{
			var pushToTalk = _chatPanel.transform.Find("PushToTalk");
			if (pushToTalk != null && pushToTalk.GetComponent<Text>())
			{
				pushToTalk.GetComponent<Text>().text = Localization.GetAndFormat("VOICE_PUSH_TO_TALK", false, "TAB");
			}
		}
	}
}