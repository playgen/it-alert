using System;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Simulation.ServerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class PlayingState : InputTickState
	{
		public const string StateName = "Playing";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public PlayingState(PlayingTickableStateInput input, Client networkClient) : base(input)
		{
			_networkClient = networkClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
		}

		protected override void OnExit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
		}

		//public override void NextState()
		//{
		//	ChangeState(FinalizingState.StateName);
		//}
		
		protected override void OnTick(float deltaTime)
		{
			ICommand command;
			if (CommandQueue.TryTakeFirstCommand(out command))
			{
				var commandResolver = new StateCommandResolver();
				commandResolver.HandleSequenceStates(command, this);
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

				// todo refactor states - move this to a transtion
				//ChangeState(FinalizingState.StateName);
				return;
			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}