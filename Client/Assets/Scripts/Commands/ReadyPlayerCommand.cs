using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Interfaces;

public class ReadyPlayerCommand : ICommand<LobbyController>
{
    private bool _ready;

    public ReadyPlayerCommand(bool ready)
    {
        _ready = ready;
    }

    public void Execute(LobbyController parameter)
    {
        if (_ready)
        {
            parameter.ReadyPlayer();
        }
        else
        {
            parameter.UnreadyPlayer();
        }
    }
}
