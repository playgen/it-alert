using System;
using Engine.Systems;
using Photon.Hive.Plugin;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Commands.Sequence;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Exceptions;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.Photon.Plugin.Analytics;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class PlayingState : RoomState
	{
		public const string StateName = "Playing";

		private readonly SimulationRoot _simulationRoot;

		private int _tickIntervalMS = 100;
		private object _tickTimer;
		
		public override string Name => StateName;

		public event Action GameOverEvent;

		public PlayingState(SimulationRoot simulationRoot, PluginBase photonPlugin, Messenger messenger,
			PlayerManager playerManager, RoomController roomController, AnalyticsServiceManager analytics)
			: base(photonPlugin, messenger, playerManager, roomController, analytics)
		{
			_simulationRoot = simulationRoot;
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

			DestroyTimer(_tickTimer);
		}

		private void ProcessGameStateMessage(Message message)
		{
			var playingMessage = message as PlayingMessage;
			if (playingMessage != null)
			{
				var player = PlayerManager.Get(playingMessage.PlayerPhotonId);
				player.State = (int) ClientState.Playing;
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == ClientState.Playing)
				{
					_tickTimer = CreateTickTimer();
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
			CommandSystem commandSystem;
			if (_simulationRoot.ECS.TryGetSystem(out commandSystem) == false)
			{
				throw new SimulationException($"Could not locate command processing system");
			}
			if (commandSystem.TryHandleCommand(command) == false)
			{
				throw new SimulationException($"Unhandled Simulation Command: ${message}");
			}
			
		}

		private void Tick()
		{
			//var commands = _commandSequence.Tick();
			//_resolver.ProcessCommands(commands);

			// TODO: reimplement tick!
			_simulationRoot.ECS.Tick();

			//if (_simulationRoot.IsGameFailure 
			//	|| (!_simulationRoot.HasViruses && !_commandSequence.HasPendingCommands))
			//{
			//	GameOverEvent();
			//}
			//else
			//{
				Messenger.SendAllMessage(new TickMessage
				{
					EntityState = _simulationRoot.GetEntityState()
				});
			//}
		}

		private object CreateTickTimer()
		{
			return PhotonPlugin.PluginHost.CreateTimer(
				Tick,
				SimulationConstants.TickInterval,
				SimulationConstants.TickInterval);
		}

		private void DestroyTimer(object timer)
		{
			PhotonPlugin.PluginHost.StopTimer(timer);
		}
	}
}
