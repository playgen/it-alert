using UnityEngine;
using System.Collections;
using GameWork.States;

public class LoginState : State
{
	private LoginStateInterface _interface;
	public const string StateName = "LoginState";

	public LoginState(LoginStateInterface @interface)
	{
		_interface = @interface;
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
			command.Execute(this);
		}
	}

	public override string Name
	{
		get { return StateName; }
	}

	public override void NextState()
	{
		throw new System.NotImplementedException();
	}
}
