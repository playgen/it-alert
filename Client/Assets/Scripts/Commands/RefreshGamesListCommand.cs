using GameWork.Commands.Interfaces;

public class RefreshGamesListCommand : ICommand<GamesListController>
{
    public void Execute(GamesListController parameter)
    {
        parameter.GetGamesList();
    }
}
