using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.Network;
using UnityEngine;

public class LobbyController : ICommandAction
{
    private readonly ITAlertClient _client;
    public event Action ReadySuccessEvent;
    public event Action<LobbyPlayer[]> RefreshSuccessEvent;

    public LobbyController(ITAlertClient client)
    {
        _client = client;
    }

    public void RefreshPlayerList()
    {
        var playerReadyStatus = _client.PlayerReadyStatus;
        var playerColors = _client.PlayerColors;
        var players = _client.ListCurrentRoomPlayers;
        var lobbyPlayers = new List<LobbyPlayer>();
        foreach (var photonPlayer in players)
        {
            var name = photonPlayer.name;
            var isReady = playerReadyStatus != null && playerReadyStatus.ContainsKey(photonPlayer.ID) && playerReadyStatus[photonPlayer.ID];
            var color = playerColors != null && playerColors.ContainsKey(photonPlayer.ID) ? playerColors[photonPlayer.ID] : "FFFFFF";
            var lobbyPlayer = new LobbyPlayer(name, isReady, photonPlayer.ID, color);
            lobbyPlayers.Add(lobbyPlayer);
        }

        RefreshSuccessEvent(lobbyPlayers.ToArray());

        if (_client.IsMasterClient)
        {
            var numReadyPlayers = playerReadyStatus.Values.Count(b => b.Equals(true));
            Debug.Log("NUMreadyPlayers: " + numReadyPlayers);
            if (numReadyPlayers == _client.CurrentRoom.maxPlayers)
            {
                Debug.Log("All Ready!");
                _client.StartGame(true); // force start = true?
            }
        }

    }

    public void LeaveLobby()
    {
        _client.QuitGame();
    }

    public void ReadyPlayer()
    {
        _client.SetReady(true);
        ReadySuccessEvent();
    }

    public void UnreadyPlayer()
    {
        _client.SetReady(false);
        ReadySuccessEvent();
    }

    public void SetColor(string colorHex)
    {
        _client.SetColor(colorHex);
        RefreshPlayerList();
    }

    public struct LobbyPlayer
    {
        public string Name;
        public bool IsReady;
        public int Id;
        public string Color;

        public LobbyPlayer(string name, bool isReady, int id, string color)
        {
            Name = name;
            IsReady = isReady;
            Id = id;
            Color = color;
        }
    }
        
}

