using System;
using GameWork.Core.Commands.Accounts.Interfaces;
using GameWork.Core.Commands.Interfaces;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Contracts.Shared;


public class LoginController : ILoginAction, ILogoutAction, ICommandAction
{
    private readonly SessionClient _sessionController;

    public event Action<string> LoginSuccessEvent;
    public event Action<string> LoginFailedEvent;

    public LoginController(SessionClient factoryAccount)
    {
        _sessionController = factoryAccount;
    }

    public void Login(string username, string password)
    {

        //LoginSuccessEvent();
        var accountRequest = new AccountRequest()
        {
            Name = username,
            Password = password
        };

        try
        {
            var accountResponse = _sessionController.CreateAndLogin(accountRequest);
            LoginSuccessEvent(accountRequest.Name);
        }
        catch (Exception ex)
        {
            LoginFailedEvent(ex.Message);
        }


    }

    public void Logout()
    {
        throw new System.NotImplementedException();
    }

}
