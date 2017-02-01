using GameWork.Core.States.Tick.Input;

namespace PlayGen.ITAlert.Unity.States.Game.Settings
{
	public class SettingsState : InputTickState
	{
		public const string StateName = nameof(SettingsState);

		public override string Name => StateName;

		public SettingsState(SettingsStateInput input) : base(input)
		{
		}
	}
}