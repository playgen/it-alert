using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.GameStates.Input;

namespace PlayGen.ITAlert.Unity.GameStates
{
	public class SettingsState : InputTickState
	{
		public const string StateName = "SettingsState";
		
		public override string Name
		{
			get { return StateName; }
		}

		public SettingsState(SettingsStateInput input) : base(input)
		{
		}
	}
}