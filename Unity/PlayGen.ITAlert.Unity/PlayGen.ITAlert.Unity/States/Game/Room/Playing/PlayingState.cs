using System.Linq;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.ITAlert.Unity.Simulation.Behaviours;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Messaging;

using Logger = PlayGen.Photon.Unity.Logger;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Playing
{
	public class PlayingState : InputTickState , ICompletable
	{
		public const string StateName = "Playing";
		public override string Name => StateName;

		private readonly Director _director;

		private readonly ITAlertPhotonClient _photonClient;
	    private readonly SimulationSummary.SimulationSummary _simulationSummary;

	    public bool IsComplete { get; private set; }

		public PlayingState(Director director, PlayingStateInput input, ITAlertPhotonClient photonClient, SimulationSummary.SimulationSummary simulationSummary) : base(input)
		{
			_director = director;
			_photonClient = photonClient;
		    _simulationSummary = simulationSummary;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			LogProxy.Info("PlayingState: OnEnter");
			_photonClient.CurrentRoom.Messenger.SendMessage(new PlayingMessage
																{
				PlayerPhotonId = _photonClient.CurrentRoom.Player.PhotonId
			});

		}

		#region Overrides of State

		protected override void OnInitialize()
		{
			LogProxy.Info("PlayingState: OnInitialize");
			
			_photonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);

			IsComplete = false;

			base.OnInitialize();
		}

		#region Overrides of State

		protected override void OnTerminate()
		{
			LogProxy.Info("PlayingState: OnTerminate");


			_photonClient.CurrentRoom.Messenger.Unsubscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);

			base.OnTerminate();
		}

		#endregion

		#endregion

		protected override void OnExit()
		{
			LogProxy.Info("PlayingState: OnExit");

		}

		private void ProcessSimulationStateMessage(Message message)
		{
			if (message is TickMessage tickMessage)
			{
				_director.UpdateSimulation(tickMessage);
				return;
			}

		    if (message is StopMessage stopMessage)
			{
                _simulationSummary.SetData(stopMessage.SimulationEvents, _director.Players.Select(p => new SimulationSummary.SimulationSummary.PlayerData
                {
                    Colour = p.Colour,
                    Name = p.Name,
                    Id = p.PhotonId
                }).ToList());
                _director.EndGame();
			}

			//throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}