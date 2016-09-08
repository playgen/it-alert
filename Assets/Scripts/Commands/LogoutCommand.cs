using GameWork.Commands.Interfaces;
using GameWork.States.Interfaces;

public class LogoutCommand : ICommand
{
    public void Execute(object parameter)
    {
        var state = (IState)parameter;
        state.ChangeState(LoginState.StateName);
    }
}
