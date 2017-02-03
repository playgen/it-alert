using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
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
		private const string PlayerChatPrefabName = "PlayerChatEntry";

		private readonly Client _photonClient;
		private GameObject _chatPanel;
		private GameObject _playerChatItemPrefab;
		private Dictionary<int, string> _playerColors;
		private Dictionary<int, Image> _playerVoiceIcons;

		public RoomStateInput(Client photonClient)
		{
			_photonClient = photonClient;
		}

		protected override void OnInitialize()
		{
			_chatPanel = GameObjectUtilities.FindGameObject("Voice/VoicePanelContainer").gameObject;
			_playerChatItemPrefab = Resources.Load(PlayerChatPrefabName) as GameObject;
		}

		protected override void OnEnter()
		{
			_photonClient.CurrentRoom.PlayerListUpdatedEvent += InitializePlayers;
		}

		protected override void OnExit()
		{
			_chatPanel.SetActive(false);
			//_photonClient.CurrentRoom.PlayerListUpdatedEvent -= InitializePlayers;
		}

		protected override void OnTick(float deltaTime)
		{
			if (_chatPanel.activeSelf)
			{
				UpdateChatPanel();
			}
		}

		private void InitializePlayers(List<Player> players)
		{
			SetPlayerColors(players);
			PopulateChatPanel(players);
		}

		private void PopulateChatPanel(ICollection<Player> players)
		{
			foreach (Transform child in _chatPanel.transform)
			{
				if (child.name.StartsWith(PlayerChatPrefabName))
				{
					Object.Destroy(child.gameObject);
				}
			}

			if (players.Count == 1)
			{
				_chatPanel.SetActive(false);
				return;
			}
			_chatPanel.SetActive(true);

			var playersList = players.OrderBy(player => player.PhotonId).ToList();

			_playerVoiceIcons = new Dictionary<int, Image>();

			foreach (var player in playersList)
			{
				var playerItem = Object.Instantiate(_playerChatItemPrefab).transform;

				Color color;
				ColorUtility.TryParseHtmlString(player.Color, out color);
				if (color == Color.white)
				{
					ColorUtility.TryParseHtmlString("#" + player.Color, out color);
				}

				var nameText = playerItem.FindChild("Name").GetComponent<Text>();
				nameText.text = player.Name;
				nameText.color = color;

				var soundIcon = playerItem.FindChild("SoundIcon").GetComponent<Image>();
				soundIcon.color = nameText.color;
				_playerVoiceIcons[player.PhotonId] = soundIcon;

				playerItem.SetParent(_chatPanel.transform, false);
			}
		}

		private void SetPlayerColors(ICollection<Player> players)
		{
			_playerColors = new Dictionary<int, string>();

			foreach (var player in players)
			{
				_playerColors.Add(player.PhotonId, player.Color);
			}
		}

		private void UpdateChatPanel()
		{
			foreach (var status in VoiceClient.TransmittingStatuses)
			{
				_playerVoiceIcons[status.Key].enabled = status.Value;
			}
		}
	}
}