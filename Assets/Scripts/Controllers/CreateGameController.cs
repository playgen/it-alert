using System;
using PlayGen.ITAlert.Network;

public class CreateGameController
{
    private ITAlertClient _client;
    public event Action CreateGameSuccessEvent;
    public event Action<string> CreateGameFailedEvent;

    public CreateGameController(ITAlertClient client)
    {
        _client = client;
    }

    public void CreateGame(string gameName, int maxPlayers)
    {
        try
        {
            _client.CreateRoom(gameName, maxPlayers);
            CreateGameSuccessEvent();
        }
        catch (Exception ex)
        {
            CreateGameFailedEvent(ex.Message);
        }
    }
}
