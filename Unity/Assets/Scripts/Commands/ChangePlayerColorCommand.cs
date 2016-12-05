using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Interfaces;

public class ChangePlayerColorCommand : ICommand<LobbyController>
{
    private string _color;

    public ChangePlayerColorCommand(string color)
    {
        _color = color;
    }

    public void Execute(LobbyController parameter)
    {
        parameter.SetColor(_color);
    }
}
