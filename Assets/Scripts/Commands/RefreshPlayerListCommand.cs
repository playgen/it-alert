
using System.Diagnostics;
using GameWork.Commands.Interfaces;

public class RefreshPlayerListCommand : ICommand
{
    public void Execute(object parameter)
    {
        var controller = (LobbyController)parameter;
        controller.RefreshPlayerList();
    }
}
