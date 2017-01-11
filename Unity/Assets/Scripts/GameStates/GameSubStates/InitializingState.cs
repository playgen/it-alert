using System;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using InitializedMessage = PlayGen.ITAlert.Photon.Messages.Simulation.States.InitializedMessage;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class InitializingState : TickState
	{
		public const string StateName = "Initializing";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public bool IsComplete
		{
			get; private set;
		}

		public InitializingState(Client networkClient)
		{
			_networkClient = networkClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);

			_networkClient.CurrentRoom.Messenger.SendMessage(new InitializingMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
			});
		}

		protected override void OnExit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
		}
		
		private void ProcessSimulationStateMessage(Message message)
		{
			var initializedMessage = message as InitializedMessage;
			if (initializedMessage != null)
			{
				var simulation = Serializer.DeserializeSimulation(initializedMessage.SerializedSimulation);
				Director.Initialize(simulation, _networkClient.CurrentRoom.Player.PhotonId);
				Director.Refresh();

				_networkClient.CurrentRoom.Messenger.SendMessage(new Photon.Messages.Game.States.InitializedMessage()
				{
					PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
				});
				return;
			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}