﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PlayGen.ITAlert.Network;

public class LobbyController
{
    private ITAlertClient _client;
    public event Action ReadySuccessEvent;
    public event Action<string> ReadyFailedEvent;
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
            var isReady = playerReadyStatus[photonPlayer.ID];
            var lobbyPlayer = new LobbyPlayer(name, isReady);
            lobbyPlayers.Add(lobbyPlayer);
        }
        RefreshSuccessEvent(lobbyPlayers.ToArray());
    }

    public void ReadyPlayer()
    {
        try
        {
            _client.SetReady(true);
            ReadySuccessEvent();
        }
        catch (Exception ex)
        {
            
        }
    }

    public void UnreadyPlayer()
    {
        try
        {
            _client.SetReady(false);
            ReadySuccessEvent();
        }
        catch (Exception ex)
        {
            
        }
    }

    public struct LobbyPlayer
    {
        public string Name;
        public bool IsReady;

        public LobbyPlayer(string name, bool isReady)
        {
            Name = name;
            IsReady = isReady;
        }
    }
        
}

