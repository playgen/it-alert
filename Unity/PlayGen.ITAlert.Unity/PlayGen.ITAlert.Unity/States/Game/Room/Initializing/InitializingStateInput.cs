using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Initializing
{
	public class InitializingStateInput : TickStateInput
	{
		protected override void OnInitialize()
		{
			GameObjectUtilities.FindGameObject("Game/Canvas/Graph");
		}


	}
}
