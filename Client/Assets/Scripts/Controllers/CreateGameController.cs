using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

public class CreateGameController : ICommandAction
{
    private Client _client;

    public CreateGameController(Client client)
    {
        _client = client;
    }

    public void CreateGame(string gameName, int maxPlayers)
    {
        _client.CreateRoom(gameName, maxPlayers);
    }
}
