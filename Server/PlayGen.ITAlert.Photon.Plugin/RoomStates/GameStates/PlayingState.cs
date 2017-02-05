﻿using System;
using Engine.Commands;
using Engine.Lifecycle;
using Engine.Systems;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.Photon.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class PlayingState : RoomState
	{
		public const string StateName = "Playing";

		private readonly SimulationLifecycleManager _simulationLifecycleManager;

		public override string Name => StateName;

		public event Action GameOverEvent;

		public PlayingState(SimulationLifecycleManager simulationLifecycleManager, PluginBase photonPlugin, Messenger messenger,
			PlayerManager playerManager,RoomSettings roomSettings, AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, roomSettings, analytics)
		{
			_simulationLifecycleManager = simulationLifecycleManager;
			// TODO: should log game exit code to analytics
			_simulationLifecycleManager.Stopped += s => OnGameOverEvent();
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)ITAlertChannel.GameState, ProcessGameStateMessage);
			Messenger.Subscribe((int)ITAlertChannel.SimulationCommand, ProcessSimulationCommandMessage);

			Messenger.SendAllMessage(new PlayingMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)ITAlertChannel.SimulationCommand, ProcessSimulationCommandMessage);
			Messenger.Unsubscribe((int)ITAlertChannel.GameState, ProcessGameStateMessage);

			_simulationLifecycleManager.Tick -= OnTick;
			_simulationLifecycleManager.TryStop();
			_simulationLifecycleManager.Dispose();
		}

		private void ProcessGameStateMessage(Message message)
		{
			var playingMessage = message as PlayingMessage;
			if (playingMessage != null)
			{
				var player = PlayerManager.Get(playingMessage.PlayerPhotonId);
				player.State = (int) ClientState.Playing;
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == ClientState.Playing 
					&& _simulationLifecycleManager.EngineState == EngineState.NotStarted)
				{
					if (_simulationLifecycleManager.TryStart())
					{
						_simulationLifecycleManager.Tick += OnTick;
					}
					else
					{
						throw new LifecycleException("Start lifecycle failed.");
					}
				}
				return;
			}
		}

		private void ProcessSimulationCommandMessage(Message message)
		{
			var commandMessage = message as CommandMessage;
			if (commandMessage == null)
			{
				throw new SimulationException($"Unhandled Simulation Command: ${message}");
			}
			var command = commandMessage.Command;
			ICommandSystem commandSystem;
			if (_simulationLifecycleManager.ECSRoot.ECS.TryGetSystem(out commandSystem) == false)
			{
				throw new SimulationException($"Could not locate command processing system");
			}
			if (commandSystem.TryHandleCommand(command) == false)
			{
				throw new SimulationException($"Unhandled Simulation Command: ${message}");
			}
			
		}

		private void OnTick()
		{
			Messenger.SendAllMessage(new TickMessage
			{
				EntityState = _simulationLifecycleManager.ECSRoot.GetEntityState()
			});
		}

		private void DestroyTimer(object timer)
		{
			PhotonPlugin.PluginHost.StopTimer(timer);
		}

		protected virtual void OnGameOverEvent()
		{
			GameOverEvent?.Invoke();
		}
	}
}
