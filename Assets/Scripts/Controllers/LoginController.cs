using System;

using GameWork.Interfaces;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts;


public class LoginController : ILoginable
{
    public event Action LoginSuccessEvent;
    public event Action<string> LoginFailedEvent;


    public void Login(string username, string password)
    {

        //LoginSuccessEvent();
        var factory = new SUGARClient("http://localhost:62312/");
        var loginController = factory.Account;

        var accountRequest = new AccountRequest()
        {
            Name = username,
            AutoLogin = true,
            Password = password
        };

        try
        {
            var accountResponse = loginController.Login(accountRequest);
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
