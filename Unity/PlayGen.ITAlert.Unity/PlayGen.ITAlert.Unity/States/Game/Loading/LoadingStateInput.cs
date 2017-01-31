using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Loading
{
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
}