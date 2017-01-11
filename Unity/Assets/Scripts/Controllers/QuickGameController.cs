using System;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Network.Client;

public class QuickGameController : ICommandAction
{
    private readonly Client _client;
    private readonly CreateGameController _createGameController;
    private readonly int _defaultMaxPlayers;

    private string UniqueGameName
    {
        get { return Guid.NewGuid().ToString().Substring(0, 7); }
    }

    public QuickGameController(Client client, CreateGameController createGameController, int defaultMaxPlayers)
    {
        _client = client;
        _createGameController = createGameController;
        _defaultMaxPlayers = defaultMaxPlayers;
    }

    public void QuickMatch()
    {
        if (0 < _client.ListRooms(ListRoomsFilters.Open).Length)
        {
            _client.JoinRandomRoom();
        }
        else
        {
            _createGameController.CreateGame(UniqueGameName, _defaultMaxPlayers);
        }
    }
}
