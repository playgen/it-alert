using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Playing
{
	public class PlayingState : InputTickState , ICompletable
	{
		public const string StateName = "Playing";
		public override string Name => StateName;

		private readonly Director _director;

		private readonly Client _photonClient;
		
		public bool IsComplete { get; private set; }

		public PlayingState(Director director, PlayingStateInput input, Client photonClient) : base(input)
		{
			_director = director;
			_photonClient = photonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			IsComplete = false;
			_photonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);


			_photonClient.CurrentRoom.Messenger.SendMessage(new PlayingMessage()
			{
				PlayerPhotonId	= _photonClient.CurrentRoom.Player.PhotonId,
			});
		}

		protected override void OnExit()
		{
			_photonClient.CurrentRoom?.Messenger.Unsubscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);
		}

		private void ProcessSimulationStateMessage(Message message)
		{
			var tickMessage = message as TickMessage;
			if (tickMessage != null)
			{
				_director.UpdateSimulation(tickMessage);
				return;
			}

			var stopMessage = message as StopMessage;
			if (stopMessage != null)
			{
				_director.EndGame();
				return;
			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}