﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Network.Client;
using PlayGen.Photon.Players;
using UnityEngine;

public class LobbyController : ICommandAction
{
    private readonly Client _client;
    public event Action ReadySuccessEvent;
    public event Action<LobbyPlayer[]> RefreshSuccessEvent;

    public LobbyController(Client client)
    {
        _client = client;
    }

    public void RefreshPlayerList()
    {
        // todo get rid of this and use the actual PlayGen.Photon.Player representation
        var lobbyPlayers = new List<LobbyPlayer>();

        foreach (var player in _client.CurrentRoom.Players)
        {
            var lobbyPlayer = new LobbyPlayer(player.Name, player.Status == PlayerStatus.Ready, player.PhotonId, player.Color);
            lobbyPlayers.Add(lobbyPlayer);
        }

        RefreshSuccessEvent(lobbyPlayers.ToArray());

        if (_client.CurrentRoom.IsMasterClient)
        {
            var numReadyPlayers = this._client.CurrentRoom.Players.Count(p => p.Status == PlayerStatus.Ready);
            Debug.Log("NUMreadyPlayers: " + numReadyPlayers);
            if (numReadyPlayers == _client.CurrentRoom.RoomInfo.maxPlayers)
            {
                Debug.Log("All Ready!");
                _client.CurrentRoom.StartGame(true); // force start = true?
            }
        }
    }

    public void LeaveLobby()
    {
        _client.CurrentRoom.Leave();
    }

    public void ReadyPlayer()
    {
        var player = _client.CurrentRoom.Player;
        player.Status = PlayerStatus.Ready;
        _client.CurrentRoom.UpdatePlayer(player);

        ReadySuccessEvent();
    }

    public void UnreadyPlayer()
    {
        var player = _client.CurrentRoom.Player;
        player.Status = PlayerStatus.NotReady;
        _client.CurrentRoom.UpdatePlayer(player);

        ReadySuccessEvent();
    }

    public void SetColor(string colorHex)
    {
        var player = _client.CurrentRoom.Player;
        player.Color = colorHex;
        _client.CurrentRoom.UpdatePlayer(player);

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

