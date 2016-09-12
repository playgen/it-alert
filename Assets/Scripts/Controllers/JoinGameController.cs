using System;
using PlayGen.ITAlert.Network;

public class JoinGameController
{
    private readonly ITAlertClient _client;
    public event Action JoinGameSuccessEvent;
    public event Action<string> JoinGameFailedEvent;

    public JoinGameController(ITAlertClient client)
    {
        _client = client;
    }

    public void QuickMatch()
    {
        try
        {
            _client.JoinRandomRoom();
            JoinGameSuccessEvent();
        }
        catch (Exception ex)
        {
            JoinGameFailedEvent(ex.Message);
        }
    }
}
