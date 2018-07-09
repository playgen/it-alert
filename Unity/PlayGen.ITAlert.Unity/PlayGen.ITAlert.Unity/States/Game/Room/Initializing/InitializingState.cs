using System;
using System.Collections.Generic;

using Engine.Serialization;
using GameWork.Core.Logging.Loggers;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.Photon.Messaging;
using Logger = PlayGen.Photon.Unity.Logger;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Initializing
{
	public class InitializingState : InputTickState
	{
		public const string StateName = "Initializing";

		private readonly Director _director;

		private readonly ITAlertPhotonClient _photonClient;

		public override string Name => StateName;

		private readonly ScenarioLoader _scenarioLoader;

		public InitializingState(Director director, InitializingStateInput input, ITAlertPhotonClient photonClient)
			: base (input)
		{
			_director = director;
			_photonClient = photonClient;
			_scenarioLoader = new ScenarioLoader();
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
			LogProxy.Info("InitializingState OnEnter");

			_photonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);
			_photonClient.CurrentRoom.Messenger.SendMessage(new InitializingMessage
																{
				PlayerPhotonId = _photonClient.CurrentRoom.Player.PhotonId
			});
            PlayGen.Unity.Utilities.Loading.Loading.Start();
        }

		protected override void OnExit()
		{
			LogProxy.Info("InitializingState OnExit");
			_photonClient.CurrentRoom.Messenger.Unsubscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);
		}
		
		private void ProcessSimulationStateMessage(Message message)
		{
			if (message is ITAlert.Photon.Messages.Simulation.States.InitializedMessage initializedMessage)
			{
				LogProxy.Info("InitializingState: Received InitializedMessage");

				if (string.IsNullOrEmpty(initializedMessage.PlayerConfiguration) 
					|| string.IsNullOrEmpty(initializedMessage.ScenarioName)
					|| _scenarioLoader.TryGetScenario(initializedMessage.ScenarioName, out var scenario) == false)
				{
					throw new InvalidOperationException("Received invalid InitializedMessage.");
				}

				try
				{
					scenario.Configuration.PlayerConfiguration = ConfigurationSerializer.Deserialize<List<PlayerConfig>>(initializedMessage.PlayerConfiguration);
					// TODO: extract simulation initialization to somewhere else
					var simulationRoot = SimulationInstaller.CreateSimulationRoot(scenario);
					if (_director.Initialize(simulationRoot, _photonClient.CurrentRoom.Player.PhotonId, _photonClient.CurrentRoom.Players))
					{
						_photonClient.CurrentRoom.Messenger.SendMessage(new InitializedMessage
																			{
							PlayerPhotonId = _photonClient.CurrentRoom.Player.PhotonId
						});
					}
				}
				catch (Exception ex)
				{
					//TODO: transition back to lobby, or show error message

					throw new SimulationException("Error creating simulation root", ex);
				}
			}
			else
			{
				throw new Exception("Unhandled Simulation State Message: " + message);
			}
		}
	}
}