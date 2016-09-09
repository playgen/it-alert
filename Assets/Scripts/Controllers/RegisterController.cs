using System;
using GameWork.Commands.Actions.Interfaces;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts;

public class RegisterController : IRegister
{
    private readonly AccountClient _accountController;

    public event Action RegisterSuccessEvent;
    public event Action<string> RegisterFailedEvent;

    public RegisterController(AccountClient accountController)
    {
        _accountController = accountController;
    }

    public void Register(string username, string password)
    {

        var accountRequest = new AccountRequest()
        {
            Name = username,
            AutoLogin = false,
            Password = password
        };

        try
        {
            var accountResponse = _accountController.Register(accountRequest);
            RegisterSuccessEvent();
        }
        catch (Exception ex)
        {
            RegisterFailedEvent(ex.Message);
        }


    }


}
