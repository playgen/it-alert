using GameWork.Core.States.Tick.Input;
public class LoadingStateInput : TickStateInput
{
	protected override void OnEnter()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanel").SetActive(true);
		// Load stuff
	}

	protected override void OnExit()
	{
		GameObjectUtilities.FindGameObject("SplashContainer/SplashPanel").SetActive(false);
	}
}
