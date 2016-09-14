using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.Network;
using UnityEngine;

public class LobbyController : ICommandAction
{
    private ITAlertClient _client;
    public event Action ReadySuccessEvent;
    public event Action<LobbyPlayer[]> RefreshSuccessEvent;

    public LobbyController(ITAlertClient client)
    {
        _client = client;
    }

    public void RefreshPlayerList()
    {
        var playerReadyStatus = _client.PlayerReadyStatus;
        var players = _client.ListCurrentRoomPlayers;
        var lobbyPlayers = new List<LobbyPlayer>();
        foreach (var photonPlayer in players)
        {
            var name = photonPlayer.name;
            var isReady = playerReadyStatus != null && playerReadyStatus.ContainsKey(photonPlayer.ID) && playerReadyStatus[photonPlayer.ID];
            var lobbyPlayer = new LobbyPlayer(name, isReady, photonPlayer.ID);
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

    public struct LobbyPlayer
    {
        public string Name;
        public bool IsReady;
        public int Id;

        public LobbyPlayer(string name, bool isReady, int id)
        {
            Name = name;
            IsReady = isReady;
            Id = id;
        }
    }
        
}

