using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.Photon.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class InitializingState : RoomState
	{
		public const string StateName = nameof(InitializingState);

		private readonly SimulationLifecycleManager _simulationLifecycleManager;
	
		public override string Name => StateName;

		public event Action<List<Player>> PlayerInitializedEvent;

		public InitializingState(SimulationLifecycleManager simulationLifecycleManager, PluginBase photonPlugin, Messenger messenger,
			PlayerManager playerManager,RoomSettings roomSettings, AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, roomSettings, analytics)
		{
			_simulationLifecycleManager = simulationLifecycleManager;
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)ITAlertChannel.GameState, ProcessGameStateMessage);	

			Messenger.SendAllMessage(new InitializingMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)ITAlertChannel.GameState, ProcessGameStateMessage);
		}

		private void ProcessGameStateMessage(Message message)
		{
			var initializingMessage = message as InitializingMessage;
			if (initializingMessage != null)
			{
				var player = PlayerManager.Get(initializingMessage.PlayerPhotonId);
				player.State = (int) ClientState.Initializing;
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == ClientState.Initializing)
				{
					Messenger.SendAllMessage(new Messages.Simulation.States.InitializedMessage
					{
						SimulationConfiguration = _simulationLifecycleManager.ECSRoot.GetConfiguration(),
						SimulationState = _simulationLifecycleManager.ECSRoot.GetEntityState(),
					});
				}
				return;
			}

			var initializedMessage = message as InitializedMessage;
			if (initializedMessage != null)
			{
				var player = PlayerManager.Get(initializedMessage.PlayerPhotonId);
				player.State = (int) ClientState.Initialized;
				PlayerManager.UpdatePlayer(player);

				PlayerInitializedEvent?.Invoke(PlayerManager.Players);
				return;
			}
		}
	}
}
