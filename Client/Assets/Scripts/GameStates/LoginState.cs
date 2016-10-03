using GameWork.Commands.Accounts;
using GameWork.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

public class LoginState : TickableSequenceState
{
	private readonly LoginStateInterface _interface;
	private readonly LoginController _loginController;
	private readonly RegisterController _registerController;
    private readonly PopupController _popupController;
    private readonly Client _client;
    public const string StateName = "LoginState";

	public LoginState(LoginStateInterface @interface, LoginController loginController, RegisterController registerController, PopupController popupController, Client client)
	{
		_interface = @interface;
		_loginController = loginController;
		_registerController = registerController;
	    _popupController = popupController;
	    _client = client;
	}

	private void OnLoginSucceeded(string name)
	{
        _client.CurrentRoom.SetPlayerName(name);
        NextState();
	}

	public override void Initialize()
	{
        _interface.Initialize();
	}

	public override void Terminate()
	{
        _interface.Terminate();
	}

	public override void Enter()
	{
        _registerController.RegisterSuccessEvent += _interface.OnRegisterSucceeded;
        _registerController.RegisterFailedEvent += _popupController.ShowErrorPopup;
        _loginController.LoginSuccessEvent += OnLoginSucceeded;
        _loginController.LoginFailedEvent += _popupController.ShowErrorPopup;
        _interface.Enter();
	}

	public override void Exit()
	{
        _registerController.RegisterSuccessEvent -= _interface.OnRegisterSucceeded;
        _registerController.RegisterFailedEvent -= _popupController.ShowErrorPopup;
        _loginController.LoginSuccessEvent -= OnLoginSucceeded;
        _loginController.LoginFailedEvent -= _popupController.ShowErrorPopup;
        _interface.Exit();
	}

	public override void PreviousState()
	{
		throw new System.NotImplementedException();
	}

	public override void Tick(float deltaTime)
	{
		if (_interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();

		    var loginCommand = command as LoginCommand;
			if (loginCommand != null)
			{
				loginCommand.Execute(_loginController);
                return;
			}

            var registerCommand = command as RegisterCommand;
            if (registerCommand != null)
            {
                registerCommand.Execute(_registerController);
                return;
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
	}

	public override string Name
	{
		get { return StateName; }
	}

	public override void NextState()
	{
		ChangeState(MenuState.StateName);
	}
}
