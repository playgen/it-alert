﻿using System;
using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Startup;
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
						Director.Initialize(simulationRoot, _networkPhotonClient.CurrentRoom.Player.PhotonId);

						_networkPhotonClient.CurrentRoom.Messenger.SendMessage(new InitializedMessage()
						{
							PlayerPhotonId = _networkPhotonClient.CurrentRoom.Player.PhotonId
						});
					}
					catch (Exception ex)
					{
						//TODO: transition back to lobby, or show error message

						throw new SimulationException("Error creating simulation root", ex);
					}
				}

			}

			throw new Exception("Unhandled Simulation State Message: " + message);
		}
	}
}