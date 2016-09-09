using System;
using GameWork.Commands.Actions.Interfaces;
using GameWork.Interfaces;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts;


public class LoginController : ILogin, ILogout
{
    private readonly AccountClient _accountController;

    public event Action LoginSuccessEvent;
    public event Action<string> LoginFailedEvent;

    public LoginController(AccountClient factoryAccount)
    {
        _accountController = factoryAccount;
    }

    public void Login(string username, string password)
    {

        //LoginSuccessEvent();
        var accountRequest = new AccountRequest()
        {
            Name = username,
            AutoLogin = true,
            Password = password
        };

        try
        {
            var accountResponse = _accountController.Login(accountRequest);
            LoginSuccessEvent();
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
