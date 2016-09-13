using GameWork.Commands.Accounts;
using GameWork.States;

public class LoginState : TickableSequenceState
{
	private LoginStateInterface _interface;
	private LoginController _loginController;
	private RegisterController _registerController;
	public const string StateName = "LoginState";

	public LoginState(LoginStateInterface @interface, LoginController loginController, RegisterController registerController)
	{
		_interface = @interface;
		_loginController = loginController;
		_registerController = registerController;
	}

	private void OnLoginSucceeded()
	{
		NextState();
	}

	public override void Initialize()
	{
		_interface.Initialize();
		_registerController.RegisterSuccessEvent += _interface.OnRegisterSucceeded;
		_registerController.RegisterFailedEvent += _interface.OnLoginFailed;
		_loginController.LoginSuccessEvent += OnLoginSucceeded;
		_loginController.LoginFailedEvent += _interface.OnLoginFailed;
	}

	public override void Terminate()
	{
		_loginController.LoginSuccessEvent -= OnLoginSucceeded;
		_loginController.LoginFailedEvent -= _interface.OnLoginFailed;
		_interface.Terminate();
	}

	public override void Enter()
	{
		_interface.Enter();
	}

	public override void Exit()
	{
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
