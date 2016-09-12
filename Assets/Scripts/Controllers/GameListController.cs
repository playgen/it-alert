using System;
using PlayGen.ITAlert.Network;
using UnityEngine;

public class GameListController
{
    private ITAlertClient _client;
    public event Action<RoomInfo[]> GameListSuccessEvent;
    public event Action<string> GameListFailedEvent;

    public GameListController(ITAlertClient client)
    {
        _client = client;
    }

    public void GetGamesList()
    {
        var rooms = _client.ListRooms(ListRoomsFilters.Open | ListRoomsFilters.Visible);
        GameListSuccessEvent(rooms);
    }

}
