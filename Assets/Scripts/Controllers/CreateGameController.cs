using System;
using PlayGen.ITAlert.Network;

public class CreateGameController
{
    private ITAlertClient _client;

    public CreateGameController(ITAlertClient client)
    {
        _client = client;
    }

    public void CreateGame(string gameName, int maxPlayers)
    {
        _client.CreateRoom(gameName, maxPlayers);
    }
}
