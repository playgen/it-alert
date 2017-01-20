using System;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Unity.Network;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.RoomSubStates
{
	public class InitializingState : TickState
	{
		public const string StateName = "Initializing";

		private readonly Client _networkPhotonClient;

		public override string Name
		{
			get { return StateName; }
		}

		public bool IsComplete
		{
			get; private set;
		}

		public InitializingState(Client networkPhotonClient)
		{
			_networkPhotonClient = networkPhotonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkPhotonClient.CurrentRoom.Messenger.Subscribe(Channel.SimulationState.IntValue(), ProcessSimulationStateMessage);

			_networkPhotonClient.CurrentRoom.Messenger.SendMessage(new InitializingMessage()
			{
				PlayerPhotonId = _networkPhotonClient.CurrentRoom.Player.PhotonId
			});
		}

		protected override void OnExit()
		{
			_networkPhotonClient.CurrentRoom.Messenger.Unsubscribe(Channel.SimulationState.IntValue(), ProcessSimulationStateMessage);
		}
		
		private void ProcessSimulationStateMessage(Message message)
		{
			var initializedMessage = message as ITAlert.Photon.Messages.Simulation.States.InitializedMessage;
			if (initializedMessage != null)
			{
				// TODO: reimplement
				//var simulation = Serializer.DeserializeSimulation(initializedMessage.SerializedSimulation);
				//Director.Initialize(simulation, _networkPhotonClient.CurrentRoom.Player.PhotonId);
				//Director.Refresh();

				_networkPhotonClient.CurrentRoom.Messenger.SendMessage(new InitializedMessage()
				{
					PlayerPhotonId = _networkPhotonClient.CurrentRoom.Player.PhotonId
				});
				return;
			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}