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
	public class PausedState : InputTickState
	{
		public const string StateName = "Paused";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public PausedState(PausedStateInput input, Client networkClient) : base(input)
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

		//public override void PreviousState()
		//{
		//	ChangeState(PlayingState.StateName);
		//}

		protected override void OnTick(float deltaTime)
		{
			ICommand command;
			if (CommandQueue.TryTakeFirstCommand(out command))
			{
				// todo this all needs to be in the transitions
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

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}