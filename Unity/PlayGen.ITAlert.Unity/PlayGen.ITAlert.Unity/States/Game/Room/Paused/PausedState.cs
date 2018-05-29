using GameWork.Core.States.Tick.Input;

using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Paused
{
	public class PausedState : InputTickState
	{
		public const string StateName = "Paused";
		public override string Name => StateName;

		public PausedState(TickStateInput input) : base(input)
		{
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
		}

		protected override void OnExit()
		{
		}

	}
}