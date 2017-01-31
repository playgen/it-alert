using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Game.Room.Paused
{
	public class PausedState : InputTickState
	{
		public const string StateName = "Paused";
		public override string Name => StateName;
		
		private readonly Client _networkPhotonClient;

		public PausedState(TickStateInput input, Client networkPhotonClient) : base(input)
		{
			_networkPhotonClient = networkPhotonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkPhotonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);
		}

		protected override void OnExit()
		{
			_networkPhotonClient.CurrentRoom.Messenger.Unsubscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);

		}
		
		private static void ProcessSimulationStateMessage(Message message)
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