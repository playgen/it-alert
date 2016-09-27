using GameWork.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class FinalizingState : TickableSequenceState
	{
		public const string StateName = "Finalizing";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public FinalizingState(Client networkClient)
		{
			_networkClient = networkClient;
		}

		public override void Enter()
		{
		}

		public override void Exit()
		{
		}

		public override void NextState()
		{
		}

		public override void PreviousState()
		{
		}

		public override void Tick(float deltaTime)
		{
			if (_networkClient.HasSimulationState)
			{
				Director.Finalise(_networkClient.TakeSimulationState());
				Director.Refresh();
				_networkClient.SetGameFinalized();
			}
		}
	}
}