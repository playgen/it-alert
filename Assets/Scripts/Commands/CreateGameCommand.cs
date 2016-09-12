using GameWork.Commands.Interfaces;

public class CreateGameCommand : ICommand
{
    private string _gameName;
    private int _maxPlayers;

    public CreateGameCommand(string gameName, int maxPlayers)
    {
        _gameName = gameName;
        _maxPlayers = maxPlayers;
    }

    public void Execute(object parameter)
    {
        var controller = (CreateGameController)parameter;
        controller.CreateGame(_gameName, _maxPlayers);
    }
}
