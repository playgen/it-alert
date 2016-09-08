using GameWork.Commands.Interfaces;

public class RefreshGamesListCommand : ICommand
{
    public void Execute(object parameter)
    {
        var controller = (GameListController) parameter;
        controller.GetGamesList();
    }
}
