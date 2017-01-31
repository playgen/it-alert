using GameWork.Core.States.Tick.Input;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Error
{
	public class ErrorState : InputTickState
	{
		public const string StateName = nameof(ErrorState);

		public override string Name => StateName;

		private readonly Client _photonClient;

		public ErrorState(ErrorStateInput input) 
			: base(input)
		{
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
		}
	}
}