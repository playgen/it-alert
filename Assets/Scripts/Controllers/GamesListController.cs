using System;
using PlayGen.ITAlert.Network;
using UnityEngine;

public class GamesListController
{
    private ITAlertClient _client;
    public event Action<RoomInfo[]> GamesListSuccessEvent;
    public event Action<string> GamesListFailedEvent;

    public GamesListController(ITAlertClient client)
    {
        _client = client;
    }

    public void GetGamesList()
    {
        var rooms = _client.ListRooms(ListRoomsFilters.Open | ListRoomsFilters.Visible);
        GamesListSuccessEvent(rooms);
    }

}
