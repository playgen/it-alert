﻿using System;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class InitializingState : RoomState
	{
		public const string StateName = "Initializing";

		private readonly Simulation.Simulation _simulation;

		public override string Name => StateName;

		public InitializingState(Simulation.Simulation simulation, PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController) 
			: base(photonPlugin, messenger, playerManager, sugarController)
		{
			_simulation = simulation;
		}

		public override void Enter()
		{
			Messenger.Subscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);

			Messenger.SendAllMessage(new Messages.Simulation.ServerState.InitializingMessage
			{
				SerializedSimulation = Serializer.SerializeSimulation(_simulation)
			});
		}

		public override void Exit()
		{
			Messenger.Unsubscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);
		}

		private void ProcessSimulationStateMessage(Message message)
		{
			var initializedMessage = message as InitializedMessage;
			if (initializedMessage != null)
			{
				// todo do in transition
				//var player = PlayerManager.Get(initializedMessage.PlayerPhotonId);
				//player.State = (int)State.Initialized;
				//PlayerManager.UpdatePlayer(player);

				//if (PlayerManager.Players.GetCombinedStates() == State.Initialized)
				//{
				//    ChangeState(PlayingState.StateName);
				//}
				//return;
			}

			throw new Exception($"Unhandled Simulation State Message: ${message}");
		}
	}
}
