﻿using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;

public class LoadingStateInterface : StateInterface {

	public override void Initialize()
	{
		
	} 

	public override void Enter()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanel").SetActive(true);
		// Load stuff
		EnqueueCommand(new NextStateCommand());
	}

	public override void Exit()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanel").SetActive(false);
	}
}