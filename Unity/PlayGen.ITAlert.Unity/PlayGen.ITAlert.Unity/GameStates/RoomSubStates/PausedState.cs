using System;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Unity.GameStates.RoomSubStates.Input;
using PlayGen.ITAlert.Unity.Network;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.RoomSubStates
{
	public class PausedState : InputTickState
	{
		public const string StateName = "Paused";

		private readonly Client _networkPhotonClient;

		public override string Name
		{
			get { return StateName; }
		}

		public PausedState(PausedStateInput input, Client networkPhotonClient) : base(input)
		{
			_networkPhotonClient = networkPhotonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkPhotonClient.CurrentRoom.Messenger.Subscribe((int)Channel.SimulationState, ProcessSimulationStateMessage);
		}

		protected override void OnExit()
		{
			_networkPhotonClient.CurrentRoom.Messenger.Unsubscribe((int)Channel.SimulationState, ProcessSimulationStateMessage);

		}
		
		private void ProcessSimulationStateMessage(Message message)
		{
			var tickMessage = message as TickMessage;
			if (tickMessage != null)
			{
				// TODO: reimplement
				//var simulation = Serializer.DeserializeSimulation(tickMessage.SerializedSimulation);
				//Director.UpdateSimulation(simulation);
				//Director.Refresh();
				return;
			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}