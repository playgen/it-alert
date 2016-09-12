using GameWork.Commands.Interfaces;
using GameWork.States.Interfaces;

public class ReadyPlayerCommand : ICommand
{
    private bool _ready;

    public ReadyPlayerCommand(bool ready)
    {
        _ready = ready;
    }

    public void Execute(object parameter)
    {
        var controller = (LobbyController)parameter;
        if (_ready)
        {
            controller.ReadyPlayer();
        }
        else
        {
            controller.UnreadyPlayer();
        }
    }
}
