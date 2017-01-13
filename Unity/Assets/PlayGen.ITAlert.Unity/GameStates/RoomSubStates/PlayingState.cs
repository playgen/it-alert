using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Interfaces;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Unity.GameStates.RoomSubStates.Input;
using PlayGen.ITAlert.Unity.Network;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.RoomSubStates
{
	public class PlayingState : InputTickState , ICompletable
	{
		public const string StateName = "Playing";

		private readonly Client _photonClient;

		public override string Name
		{
			get { return StateName; }
		}

		public bool IsComplete
		{
			get; private set;
		}

		public PlayingState(PlayingStateInput input, Client photonClient) : base(input)
		{
			_photonClient = photonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			IsComplete = false;
			_photonClient.CurrentRoom.Messenger.Subscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);

			_photonClient.CurrentRoom.Messenger.SendMessage(new PlayingMessage()
			{
				PlayerPhotonId	= _photonClient.CurrentRoom.Player.PhotonId,
			});
		}

		protected override void OnExit()
		{
			_photonClient.CurrentRoom.Messenger.Unsubscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);
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