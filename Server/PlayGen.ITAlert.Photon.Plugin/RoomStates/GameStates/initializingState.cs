using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.Photon.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class InitializingState : ITAlertRoomState
	{
		public const string StateName = nameof(InitializingState);

		private readonly SimulationLifecycleManager _simulationLifecycleManager;
	
		public override string Name => StateName;

		public event Action<List<ITAlertPlayer>> PlayersInitialized;

		public InitializingState(SimulationLifecycleManager simulationLifecycleManager, 
			PluginBase photonPlugin, 
			Messenger messenger,
			ITAlertPlayerManager playerManager,
			RoomSettings roomSettings, 
			AnalyticsServiceManager analytics)
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
			if (message is InitializingMessage initializingMessage)
			{
				var player = PlayerManager.Get(initializingMessage.PlayerPhotonId);
				player.State = (int) ClientState.Initializing;
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == ClientState.Initializing)
				{
					Messenger.SendAllMessage(new Messages.Simulation.States.InitializedMessage
					{
						PlayerConfiguration = _simulationLifecycleManager.ECSRoot.GetPlayerConfiguration(),
						SimulationState = _simulationLifecycleManager.ECSRoot.GetEntityState(),
						ScenarioName = RoomSettings.GameScenario,
						InstanceId = _simulationLifecycleManager.ECSRoot.InstanceId
					});
				}
				return;
			}

			if (message is InitializedMessage initializedMessage)
			{
				var player = PlayerManager.Get(initializedMessage.PlayerPhotonId);
				player.State = (int) ClientState.Initialized;
				PlayerManager.UpdatePlayer(player);

				OnPlayerInitialized(PlayerManager.Players);
			}
		}

		protected virtual void OnPlayerInitialized(List<ITAlertPlayer> players)
		{
			PlayersInitialized?.Invoke(players);
		}
	}
}
