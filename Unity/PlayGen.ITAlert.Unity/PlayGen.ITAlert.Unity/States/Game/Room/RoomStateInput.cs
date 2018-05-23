using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Scoring.Player;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Unity.Client.Voice;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;
using UnityEngine.UI;

using Object = UnityEngine.Object;

namespace PlayGen.ITAlert.Unity.States.Game.Room
{
	public class RoomStateInput : TickStateInput
	{
		private class PlayerVoiceItem
		{
			public GameObject GameObject { get; set; }

			public Image VoiceIcon { get; set; }

			public Image PlayerGlyph { get; set; }

			public Image PlayerColor { get; set; }

			public Text NameText { get; set; }

			public Text ResourceText { get; set; }

			public Text SystemText { get; set; }
		}

		private Dictionary<int, PlayerVoiceItem> _playerVoiceItems;
		private Dictionary<int, int> _playerIdPair;

		private readonly ITAlertPhotonClient _photonClient;
		private GameObject _chatPanel;
		private GameObject _playerChatItemPrefab;

		public Director Director { get; }
		private PlayerScoringSystem _scoringSystem;

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
			_chatPanel.SetActive(true);

			foreach (var playerVoiceItem in _chatPanel.transform)
			{
				if (((Transform)playerVoiceItem).gameObject.name != "PushToTalk")
				{
					Object.Destroy(((Transform)playerVoiceItem).gameObject);
				}
			}
			_playerVoiceItems.Clear();
			_scoringSystem = null;
			Director.Reset += SetScoringSystem;
		}

		protected override void OnExit()
		{
			_chatPanel.SetActive(false);
			Director.Reset -= SetScoringSystem;
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
			foreach (var player in players)
			{
				if (_playerVoiceItems.TryGetValue(player.PhotonId, out var playerVoiceItem) == false)
				{
					var playerItem = Object.Instantiate(_playerChatItemPrefab);


					var nameText = playerItem.transform.Find("Name").GetComponent<Text>();
					nameText.text = player.Name;

					var resourceText = playerItem.transform.Find("Resource Management").GetComponent<Text>();

					var systemText = playerItem.transform.Find("Systematicity").GetComponent<Text>();

					var soundIcon = playerItem.transform.Find("SoundIcon").GetComponent<Image>();

					var playerGlyph = playerItem.transform.Find("Glyph").GetComponent<Image>();

					var playerColor = playerItem.transform.Find("Color").GetComponent<Image>();

					playerItem.transform.SetParent(_chatPanel.transform, false);

					playerVoiceItem = new PlayerVoiceItem
										{
						GameObject = playerItem,
						VoiceIcon = soundIcon,
						PlayerGlyph = playerGlyph,
						PlayerColor = playerColor,
						NameText = nameText,
						ResourceText = resourceText,
						SystemText = systemText
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
			_playerIdPair = GameObjectUtilities.FindGameObject("Game").GetComponentsInChildren<PlayerBehaviour>().ToDictionary(p => p.PhotonId, p => p.Id);
		}

		private void UpdatePlayerVoiceItem(ITAlertPlayer player, PlayerVoiceItem playerVoiceItem)
		{
			if (ColorUtility.TryParseHtmlString(player.Colour, out var colour))
			{
				playerVoiceItem.NameText.color = colour;
				playerVoiceItem.VoiceIcon.color = colour;
				playerVoiceItem.PlayerColor.color = colour;
			}
			playerVoiceItem.PlayerGlyph.sprite = Resources.Load<Sprite>($"playerglyph_{player.Glyph}");
			if (playerVoiceItem.NameText.text != player.Name)
			{
				playerVoiceItem.NameText.text = player.Name;
			}
		}

		private void SetScoringSystem()
		{
			if (Director.SimulationRoot.ECS.TryGetSystem(out _scoringSystem) == false)
			{
				throw new SimulationIntegrationException("Could not locate player scoring system");
			}
		}

		private void UpdateChatPanel()
		{
			foreach (var player in _playerVoiceItems)
			{
				if (player.Value != null && player.Value.ResourceText != null && player.Value.SystemText != null && player.Value.VoiceIcon != null)
				{
					if (_scoringSystem != null)
					{
						if (_playerIdPair.ContainsKey(player.Key))
						{
							var score = _scoringSystem.GetScoreForPlayerEntity(_playerIdPair[player.Key]);
							player.Value.ResourceText.text = score.ResourceManagement.ToString();
							player.Value.SystemText.text = score.Systematicity.ToString();
						}
					}
					if (VoiceClient.TransmittingStatuses.ContainsKey(player.Key))
					{
						player.Value.VoiceIcon.enabled = VoiceClient.TransmittingStatuses[player.Key];
					}
				}
			}
			_chatPanel.BestFit();
		}

		private void OnLanguageChange()
		{
			var pushToTalk = _chatPanel.transform.Find("PushToTalk/PushToTalk Text");
			if (pushToTalk != null && pushToTalk.GetComponent<Text>())
			{
				pushToTalk.GetComponent<Text>().text = Localization.GetAndFormat("VOICE_PUSH_TO_TALK", false, "TAB");
			}
		}
	}
}