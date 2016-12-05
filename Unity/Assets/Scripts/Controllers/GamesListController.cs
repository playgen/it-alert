using System;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

public class GamesListController : ICommandAction
{
    private readonly Client _client;
    public event Action<RoomInfo[]> GamesListSuccessEvent;
    public event Action<string> GamesListFailedEvent;

    public GamesListController(Client client)
    {
        _client = client;
    }

    public void GetGamesList()
    {
        var rooms = _client.ListRooms(ListRoomsFilters.Open | ListRoomsFilters.Visible);
        GamesListSuccessEvent(rooms);
    }

}
