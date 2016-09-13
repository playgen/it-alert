
using System.Diagnostics;
using GameWork.Commands.Interfaces;

public class RefreshPlayerListCommand : ICommand<LobbyController>
{
    public void Execute(LobbyController parameter)
    {
        parameter.RefreshPlayerList();
    }
}
