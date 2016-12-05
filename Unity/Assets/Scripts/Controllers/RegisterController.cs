using System;
using GameWork.Core.Commands.Accounts.Interfaces;
using GameWork.Core.Commands.Interfaces;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Contracts.Shared;

public class RegisterController : IRegisterAction, ICommandAction
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
            Password = password
        };

        try
        {
            var accountResponse = _accountController.Create(accountRequest);
            RegisterSuccessEvent();
        }
        catch (Exception ex)
        {
            RegisterFailedEvent(ex.Message);
        }


    }


}
