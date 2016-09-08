using System;

public class ReadyPlayerController
{
    public event Action ReadySuccessEvent;
    public event Action<string> ReadyFailedEvent;

    public void ReadyPlayer()
    { 
        ReadySuccessEvent();
    }

    public void UnreadyPlayer()
    {
        ReadySuccessEvent();
    }
        
}

