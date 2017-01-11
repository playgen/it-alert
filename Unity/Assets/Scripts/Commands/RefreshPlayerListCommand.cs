using GameWork.Core.Commands.Interfaces;

public class RefreshPlayerListCommand : ICommand<LobbyController>
{
    public void Execute(LobbyController parameter)
    {
        parameter.RefreshPlayerList();
    }
}
