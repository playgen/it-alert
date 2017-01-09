using System;
using GameWork.Core.States;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Simulation.ServerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class PlayingState : TickState
	{
		private readonly PlayingTickableStateInterface _interface;
		public const string StateName = "Playing";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public PlayingState(PlayingTickableStateInterface @interface, Client networkClient)
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
			Logger.LogDebug("Entered " + StateName);

			_interface.Enter();
			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
		}

		public override void Exit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
			_interface.Exit();
		}

		//public override void NextState()
		//{
		// todo refactor states
		//	ChangeState(FinalizingState.StateName);
		//}
		
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
		}

		private void ProcessSimulationStateMessage(Message message)
		{
			var tickMessage = message as TickMessage;
			if (tickMessage != null)
			{
				var simulation = Serializer.DeserializeSimulation(tickMessage.SerializedSimulation);
				Director.UpdateSimulation(simulation);
				Director.Refresh();
				return;
			}

			var finalizingMessage = message as FinalizingMessage;
			if (finalizingMessage != null)
			{
				var simulation = Serializer.DeserializeSimulation(finalizingMessage.SerializedSimulation);
				Director.UpdateSimulation(simulation);
				Director.Refresh();

				// todo refactor states
				//ChangeState(FinalizingState.StateName);
				return;
			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}