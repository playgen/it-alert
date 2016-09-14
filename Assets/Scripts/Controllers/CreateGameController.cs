using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.Network;

public class CreateGameController : ICommandAction
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
