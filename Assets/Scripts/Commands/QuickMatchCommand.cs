using GameWork.Commands.Interfaces;

public class QuickMatchCommand : ICommand<JoinGameController>
{
    public void Execute(JoinGameController parameter)
    {
        parameter.QuickMatch();
    }
}
