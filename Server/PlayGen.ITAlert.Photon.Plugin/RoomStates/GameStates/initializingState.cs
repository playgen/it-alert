using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Startup;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class InitializingState : RoomState
	{
		public const string StateName = "Initializing";

		private readonly SimulationRoot _simulationRoot;
	
		public override string Name => StateName;

		public event Action<List<Player>> PlayerInitializedEvent;

		public InitializingState(SimulationRoot simulationRoot, 
			PluginBase photonPlugin, 
			Messenger messenger, 
			PlayerManager playerManager, 
			Controller sugarController) 
			: base(photonPlugin, messenger, playerManager, sugarController)
		{
			_simulationRoot = simulationRoot;
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)Channel.GameState, ProcessGameStateMessage);	

			Messenger.SendAllMessage(new InitializingMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)Channel.GameState, ProcessGameStateMessage);
		}

		private void ProcessGameStateMessage(Message message)
		{
			var initializingMessage = message as InitializingMessage;
			if (initializingMessage != null)
			{
				var player = PlayerManager.Get(initializingMessage.PlayerPhotonId);
				player.State = ClientState.Initializing.IntValue();
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == ClientState.Initializing)
				{
					Messenger.SendAllMessage(new Messages.Simulation.States.InitializedMessage
					{
						SimulationConfiguration = _simulationRoot.GetConfiguration(),
						SimulationState = _simulationRoot.GetEntityState(),
					});
				}
				return;
			}

			var initializedMessage = message as InitializedMessage;
			if (initializedMessage != null)
			{
				var player = PlayerManager.Get(initializedMessage.PlayerPhotonId);
				player.State = ClientState.Initialized.IntValue();
				PlayerManager.UpdatePlayer(player);

				PlayerInitializedEvent(PlayerManager.Players);
				return;
			}
		}
	}
}
