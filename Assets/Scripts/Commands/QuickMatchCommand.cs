using GameWork.Commands.Interfaces;

public class QuickMatchCommand : ICommand
{
    public void Execute(object parameter)
    {
        var controller = (JoinGameController)parameter;
        controller.QuickMatch();
    }
}
