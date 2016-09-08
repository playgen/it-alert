using System;

public class GameListController
{

    public event Action<object[]> GameListSuccessEvent;
    public event Action<string> GameListFailedEvent;

    public void GetGamesList()
    {
        GameListSuccessEvent(new object[] {});
    }

}
