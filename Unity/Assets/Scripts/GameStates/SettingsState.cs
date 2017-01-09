﻿using System;
using GameWork.Core.States;

public class SettingsState : TickState
{
	private readonly SettingsStateInterface _interface;

	public const string StateName = "SettingsState";


	public SettingsState(SettingsStateInterface @interface)
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
		_interface.Tick(deltaTime);
		if (_interface.HasCommands)
		{
			var command = _interface.TakeFirstCommand();

			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
	}

	public override string Name
	{
		get { return StateName; }
	}
	
	// todo refactor states
	//public override void PreviousState()
	//{
	//	BackState();
	//}
}
