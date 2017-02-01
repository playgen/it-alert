using GameWork.Core.States.Tick.Input;

namespace PlayGen.ITAlert.Unity.States.Error
{
	public class ErrorState : InputTickState
	{
		public const string StateName = nameof(ErrorState);

		public override string Name => StateName;

		public ErrorState(ErrorStateInput input) 
			: base(input)
		{
		}
	}
}