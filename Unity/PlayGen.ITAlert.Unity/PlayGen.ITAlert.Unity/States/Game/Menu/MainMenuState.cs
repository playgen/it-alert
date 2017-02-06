using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Commands;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.States.Game.Menu
{
	public class MainMenuState : InputTickState
	{
		public const string StateName = nameof(MainMenuState);

		public override string Name => StateName;
		
		public MainMenuState(MenuStateInput input) : base(input)
		{
		}
	}
}