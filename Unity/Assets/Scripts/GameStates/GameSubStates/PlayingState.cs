using System;
using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Interfaces;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
	public class PlayingState : InputTickState , ICompletable
	{
		public const string StateName = "Playing";

		private readonly Client _networkClient;

		public override string Name
		{
			get { return StateName; }
		}

		public bool IsComplete
		{
			get; private set;
		}

		public PlayingState(PlayingTickableStateInput input, Client networkClient) : base(input)
		{
			_networkClient = networkClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			IsComplete = false;
			_networkClient.CurrentRoom.Messenger.Subscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);

			_networkClient.CurrentRoom.Messenger.SendMessage(new PlayingMessage()
			{
				PlayerPhotonId = _networkClient.CurrentRoom.Player.PhotonId
			});
		}

		protected override void OnExit()
		{
			_networkClient.CurrentRoom.Messenger.Unsubscribe((int)Photon.Messages.Channels.SimulationState, ProcessSimulationStateMessage);
		}
		
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
			var initializedMessage = message as InitializedMessage;
			if (initializedMessage != null)
			{
				var simulation = Serializer.DeserializeSimulation(initializedMessage.SerializedSimulation);
				Director.Initialize(simulation, _networkClient.CurrentRoom.Player.PhotonId);
				Director.Refresh();

				return;
			}

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