using GameWork.Commands.Interfaces;

public class QuickGameCommand : ICommand<QuickGameController>
{
    public void Execute(QuickGameController parameter)
    {
        parameter.QuickMatch();
    }
}
