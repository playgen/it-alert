﻿using System;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Initializing
{
	public class InitializingState : TickState
	{
		public const string StateName = "Initializing";

		private readonly Client _photonClient;

		public override string Name => StateName;

		public InitializingState(Client photonClient)
		{
			_photonClient = photonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);

			_photonClient.CurrentRoom.Messenger.Subscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);
			_photonClient.CurrentRoom.Messenger.SendMessage(new InitializingMessage()
			{
				PlayerPhotonId = _photonClient.CurrentRoom.Player.PhotonId
			});
		}

		protected override void OnExit()
		{
			_photonClient.CurrentRoom.Messenger.Unsubscribe((int)ITAlertChannel.SimulationState, ProcessSimulationStateMessage);
		}
		
		private void ProcessSimulationStateMessage(Message message)
		{
			var initializedMessage = message as ITAlert.Photon.Messages.Simulation.States.InitializedMessage;
			if (initializedMessage != null)
			{
				if (Director.Initialized == false)
				{
					if (string.IsNullOrEmpty(initializedMessage.SimulationConfiguration))
					{
						throw new InvalidOperationException("Received InitializedMessage without configuration.");
					}

					try
					{
						// TODO: extract simulation initialization to somewhere else
						var simulationRoot = SimulationInstaller.CreateSimulationRoot(initializedMessage.SimulationConfiguration);
						if (Director.Initialize(simulationRoot, _photonClient.CurrentRoom.Player.PhotonId, _photonClient.CurrentRoom.Players))
						{
							_photonClient.CurrentRoom.Messenger.SendMessage(new InitializedMessage()
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
			}
			else
			{
				throw new Exception("Unhandled Simulation State Message: " + message);
			}
		}
	}
}