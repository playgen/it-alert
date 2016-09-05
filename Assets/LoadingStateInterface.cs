using GameWork.Commands;
using UnityEngine;
using GameWork.Interfacing;

public class LoadingStateInterface : StateInterface {

	public override void Initialize()
	{
		
	} 

	public override void Enter()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashImage").SetActive(true);
		// Load stuff
		EnqueueCommand(new NextStateCommand());
	}

	public override void Exit()
	{
	}
}
