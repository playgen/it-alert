using System;
using System.Collections.Generic;
using GameWork.Core.States;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Messages.Simulation.PlayerState;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Players.Extensions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class FinalizingState : RoomState
	{
		public const string StateName = "Finalizing";

		private readonly Simulation.Simulation _simulation;

		public override string Name => StateName;

		public event Action<List<Player>> PlayerFinalizedEvent;

		public FinalizingState(Simulation.Simulation simulation, PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController) 
			: base(photonPlugin, messenger, playerManager, sugarController)
		{
			_simulation = simulation;
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);

			Messenger.SendAllMessage(new Messages.Simulation.ServerState.FinalizingMessage
			{
				SerializedSimulation = Serializer.SerializeSimulation(_simulation)
			});
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)Channels.SimulationState, ProcessSimulationStateMessage);
		}

		private void ProcessSimulationStateMessage(Message message)
		{
			var finalizedMessage = message as FinalizedMessage;
			if (finalizedMessage != null)
			{
				var player = PlayerManager.Get(finalizedMessage.PlayerPhotonId);
				player.State = (int)State.Finalized;
				PlayerManager.UpdatePlayer(player);
				
				PlayerFinalizedEvent(PlayerManager.Players);
				return;
			}

			throw new Exception($"Unhandled Simulation State Message: ${message}");
		}
	}
}
