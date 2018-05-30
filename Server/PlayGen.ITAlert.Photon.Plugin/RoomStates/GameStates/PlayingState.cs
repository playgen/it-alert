using System;

using Engine.Events;
using Engine.Lifecycle;
using Engine.Serialization;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Plugin;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Game.UI;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.ITAlert.Simulation.UI.Events;
using PlayGen.Photon.Analytics;
using System.Collections.Generic;
using PlayGen.ITAlert.Simulation.Logging;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class PlayingState : ITAlertRoomState, IDisposable
	{
		public const string StateName = nameof(PlayingState);

		private readonly SimulationLifecycleManager _simulationLifecycleManager;

		public override string Name => StateName;

		public PlayingState(SimulationLifecycleManager simulationLifecycleManager,
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
			Messenger.Subscribe((int)ITAlertChannel.SimulationCommand, ProcessSimulationCommandMessage);
			Messenger.Subscribe((int)ITAlertChannel.UiEvent, ProcessUiEventMessage);

			Messenger.SendAllMessage(new PlayingMessage());
		}

		private void ProcessUiEventMessage(Message message)
		{
			if (message is UIEventMessage uiEventMessage)
			{
				switch (uiEventMessage)
				{
					case PlayerVoiceActivatedMessage pva:
						PublishSimulationEvent(new PlayerVoiceEvent
						{
							Mode = PlayerVoiceEvent.Signal.Activated,
							PlayerEntityId = pva.PlayerId
						});
						break;

					case PlayerVoiceDeactivatedMessage pvd:
						PublishSimulationEvent(new PlayerVoiceEvent
						{
							Mode = PlayerVoiceEvent.Signal.Deactivated,
							PlayerEntityId = pvd.PlayerId
						});
						break;
				}

			}
		}

		private void PublishSimulationEvent(IEvent @event)
		{
			if (_simulationLifecycleManager.ECSRoot.ECS.TryGetSystem<EventSystem>(out var eventSystem))
			{
				eventSystem.Publish(@event);
			}
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)ITAlertChannel.SimulationCommand, ProcessSimulationCommandMessage);
			Messenger.Unsubscribe((int)ITAlertChannel.GameState, ProcessGameStateMessage);
			Messenger.Unsubscribe((int)ITAlertChannel.UiEvent, ProcessUiEventMessage);

			Shutdown(false);
		}

		private void Shutdown(bool dispose)
		{
			_simulationLifecycleManager.Tick -= OnTick;
			_simulationLifecycleManager.TryStop();

			if (dispose)
			{
				_simulationLifecycleManager.Dispose();
			}
		}

		private void ProcessGameStateMessage(Message message)
		{
			if (message is PlayingMessage playingMessage)
			{
				if (_simulationLifecycleManager.EngineState == EngineState.NotStarted)
				{
					var player = PlayerManager.Get(playingMessage.PlayerPhotonId);
					player.State = (int)ClientState.Playing;
					PlayerManager.UpdatePlayer(player);

					PlayingStateCheck();
				}
			}
		}

		private void PlayingStateCheck()
		{
			if (PlayerManager.Players.GetCombinedStates() == ClientState.Playing
				&& _simulationLifecycleManager.EngineState == EngineState.NotStarted)
			{
				if (_simulationLifecycleManager.TryStart())
				{
					_simulationLifecycleManager.Tick += OnTick;
					_simulationLifecycleManager.Stopped += SimulationLifecycleManagerOnStopped;
				}
				else
				{
					throw new LifecycleException("Start lifecycle failed.");
				}
			}
		}

		private void SimulationLifecycleManagerOnStopped(ExitCode exitCode)
		{
			List<StopMessage.SimulationEvent> simulationEvents = null;

			// If logging was enabled, include events for the game
			if (_simulationLifecycleManager.ECSRoot.ECS.GetSystems<DatabaseEventLogger>().Any())
			{
				var gameId = _simulationLifecycleManager.ECSRoot.InstanceId;
				using (var loggingController = new ITAlertLoggingController())
				{
					var events = loggingController.GetGameEvents(gameId);
					simulationEvents = events.Select(e => new StopMessage.SimulationEvent
					{
						PlayerId = e.PlayerExternalId,
						Data = e.Data,
						EventCode = e.EventCode,
						Tick = e.Tick

					}).ToList();
				}
			}

			Messenger.SendAllMessage(new StopMessage
			{
				SimulationEvents = simulationEvents
			});
		}

		private void ProcessSimulationCommandMessage(Message message)
		{
			if (!(message is CommandMessage commandMessage))
			{
				throw new SimulationException($"Unhandled Simulation Command: ${message}");
			}
			var command = commandMessage.Command;
			_simulationLifecycleManager.EnqueueCommand(command);
		}

		private void OnTick(Tick tick, uint crc)
		{
			var tickString = ConfigurationSerializer.Serialize(tick);

			Messenger.SendAllMessage(new TickMessage
			{
				CRC = crc,
				TickString = tickString
			});
		}


		public override void OnLeave(ILeaveGameCallInfo info)
		{
			if (PlayerManager.Players.Count == 0)
			{
				Shutdown(true);
			}
			else
			{
				_simulationLifecycleManager.ECSRoot.ECS.PlayerDisconnected(info.ActorNr - 1);
				PlayingStateCheck();
			}
			base.OnLeave(info);
		}

		public void Dispose()
		{
			_simulationLifecycleManager?.Dispose();
		}
	}
}
