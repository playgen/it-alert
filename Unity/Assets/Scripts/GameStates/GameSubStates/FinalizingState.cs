using GameWork.Core.States;
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
			// todo have reference to simulation state object

			//if (_networkClient.CurrentRoom.CurrentGame.HasSimulationState)
			//{
			//	Director.Finalise(_networkClient.CurrentRoom.CurrentGame.TakeSimulationState());
			//	Director.Refresh();
			//	_networkClient.CurrentRoom.CurrentGame.SetGameFinalized();
			//}
		}
	}
}