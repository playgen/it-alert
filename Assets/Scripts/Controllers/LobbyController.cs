using System;
using PlayGen.ITAlert.Network;

public class LobbyController
{
    private ITAlertClient _client;
    public event Action ReadySuccessEvent;
    public event Action<string> ReadyFailedEvent;

    public LobbyController(ITAlertClient client)
    {
        _client = client;
    }

    public void ReadyPlayer()
    {
        try
        {
            _client.SetReady(true);
            ReadySuccessEvent();
        }
        catch (Exception ex)
        {
            
        }
    }

    public void UnreadyPlayer()
    {
        try
        {
            _client.SetReady(false);
            ReadySuccessEvent();
        }
        catch (Exception ex)
        {
            
        }
    }
        
}

