using GameWork.Core.Interfacing;
using GameWork.Core.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class PausedState : TickableSequenceState
	{
		private readonly PausedStateInterface _interface;
		public const string StateName = "Paused";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public PausedState(PausedStateInterface @interface, Client networkClient)
		{
			_interface = @interface;
			_networkClient = networkClient;
		}

		public override void Initialize()
		{
			_interface.Initialize();
		}

		public override void Enter()
		{
			_interface.Enter();
		}

		public override void Exit()
		{
			_interface.Exit();
		}

		public override void NextState()
		{
			
		}

		public override void PreviousState()
		{
			ChangeState(PlayingState.StateName);
		}

		public override void Tick(float deltaTime)
		{
			_interface.Tick(deltaTime);
			if (_interface.HasCommands)
			{
				if (_interface.HasCommands)
				{
					var command = _interface.TakeFirstCommand();

					var commandResolver = new StateCommandResolver();
					commandResolver.HandleSequenceStates(command, this);
				}
			}
			if (_networkClient.CurrentRoom.CurrentGame.HasSimulationState)
			{
				Director.UpdateSimulation(_networkClient.CurrentRoom.CurrentGame.TakeSimulationState());
				Director.Refresh();
			}
		}
	}
}