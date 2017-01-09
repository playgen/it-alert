using GameWork.Legacy.Core.Interfacing;

public class LoadingStateInterface : TickableStateInterface {

	public override void Initialize()
	{
		
	} 

	public override void Enter()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanel").SetActive(true);
		// Load stuff
		// todo refactor states
		//EnqueueCommand(new NextStateCommand());
	}

	public override void Exit()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanel").SetActive(false);
	}
}
