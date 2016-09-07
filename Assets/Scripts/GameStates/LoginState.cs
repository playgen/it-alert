using GameWork.States;

public class LoginState : State
{
	private LoginStateInterface _interface;
	private LoginController _controller;
	public const string StateName = "LoginState";

	public LoginState(LoginStateInterface @interface, LoginController controller)
	{
		_interface = @interface;
		_controller = controller;
		_controller.LoginSuccessEvent += OnLoginSucceeded;
		_controller.LoginFailedEvent += _interface.OnLoginFailed;
	}

	private void OnLoginSucceeded()
	{
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
		_interface.Enter();
	}

	public override void Exit()
	{
		_interface.Exit();
	}

	public override void Tick(float deltaTime)
	{
		if (_interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();
			if (command is LoginCommand)
			{
				command.Execute(_controller);
			}
			else
			{
				command.Execute(this);
			}
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
