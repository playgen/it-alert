using GameWork.Commands.Interfaces;
using GameWork.Interfaces;

public struct LoginCommand : ICommand
{
    private readonly string _password;
    private readonly string _username;

    public LoginCommand(string user, string pass)
    {
        _username = user;
        _password = pass;
    }

    public void Execute(object parameter)
    {
        var loginable = (ILoginable)parameter;
        loginable.Login(_username, _password);
    }
}

