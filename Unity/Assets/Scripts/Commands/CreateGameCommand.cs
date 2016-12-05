using GameWork.Core.Commands.Interfaces;

public class CreateGameCommand : ICommand<CreateGameController>
{
    private string _gameName;
    private int _maxPlayers;

    public CreateGameCommand(string gameName, int maxPlayers)
    {
        _gameName = gameName;
        _maxPlayers = maxPlayers;
    }

    public void Execute(CreateGameController parameter)
    {
        parameter.CreateGame(_gameName, _maxPlayers);
    }
}
