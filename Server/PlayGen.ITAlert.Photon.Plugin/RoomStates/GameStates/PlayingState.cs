using System;
using Engine.Commands;
using Engine.Events;
using Engine.Lifecycle;
using Engine.Serialization;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
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

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class PlayingState : RoomState, IDisposable
	{
		public const string StateName = nameof(PlayingState);

		private readonly SimulationLifecycleManager _simulationLifecycleManager;

		public override string Name => StateName;

		public PlayingState(SimulationLifecycleManager simulationLifecycleManager, 
			PluginBase photonPlugin, 
			Messenger messenger,
			PlayerManager playerManager,
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
			var uiEventMessage = message as UIEventMessage;
			if (uiEventMessage != null)
			{
				switch (uiEventMessage)
				{
					case PlayerVoiceActivatedMessage pva:
						PublishSimulationEvent(new PlayerVoiceEvent()
						{
							Mode = PlayerVoiceEvent.Signal.Activated,
							PlayerEntityId = pva.PlayerId,
						});
						break;

					case PlayerVoiceDeactivatedMessage pvd:
						PublishSimulationEvent(new PlayerVoiceEvent() {
							Mode = PlayerVoiceEvent.Signal.Deactivated,
							PlayerEntityId = pvd.PlayerId,
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
			switch (_simulationLifecycleManager.EngineState)
			{
				case EngineState.Error:
				case EngineState.Stopped:
					return;
			}

			_simulationLifecycleManager.Tick -= OnTick;
			_simulationLifecycleManager.TryStop();

			if (dispose)
			{
				_simulationLifecycleManager.Dispose();
			}
		}

		private void ProcessGameStateMessage(Message message)
		{
			var playingMessage = message as PlayingMessage;
			if (playingMessage != null)
			{
				if (_simulationLifecycleManager.EngineState == EngineState.NotStarted)
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
							_simulationLifecycleManager.Stopped += SimulationLifecycleManagerOnStopped;
						}
						else
						{
							throw new LifecycleException("Start lifecycle failed.");
						}
					}
					return;
				}
			}
		}

		private void SimulationLifecycleManagerOnStopped(ExitCode exitCode)
		{
			Messenger.SendAllMessage(new StopMessage());
		}

		private void ProcessSimulationCommandMessage(Message message)
		{
			var commandMessage = message as CommandMessage;
			if (commandMessage == null)
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
				TickString = tickString,
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
				_simulationLifecycleManager.ECSRoot.ECS.PlayerDisconnected(info.ActorNr);
			}
			base.OnLeave(info);
		}

		public void Dispose()
		{
			_simulationLifecycleManager?.Dispose();
		}
	}
}
