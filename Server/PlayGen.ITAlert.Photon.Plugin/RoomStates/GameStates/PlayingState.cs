using System;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Sequence;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Startup;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class PlayingState : RoomState
	{
		public const string StateName = "Playing";

		private readonly SimulationRoot _simulationRoot;

		//private CommandSequence _commandSequence;
		private CommandResolver _resolver;
		private int _tickIntervalMS = 100;
		private object _tickTimer;
		
		public override string Name => StateName;

		public event Action GameOverEvent;

		public PlayingState(SimulationRoot simulationRoot, 
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
			Messenger.Subscribe((int)Channel.SimulationCommand, ProcessSimulationCommandMessage);

			//_commandSequence = CommandSequenceHelper.GenerateCommandSequence(20, 20, 40);// todo uncomment: 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
			_resolver = new CommandResolver(_simulationRoot.ECS);

			Messenger.SendAllMessage(new PlayingMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)Channel.SimulationCommand, ProcessSimulationCommandMessage);
			Messenger.Unsubscribe((int)Channel.GameState, ProcessGameStateMessage);

			DestroyTimer(_tickTimer);
			_resolver = null;
			//_commandSequence = null;
		}

		private void ProcessGameStateMessage(Message message)
		{
			var playingMessage = message as PlayingMessage;
			if (playingMessage != null)
			{
				var player = PlayerManager.Get(playingMessage.PlayerPhotonId);
				player.State = State.Playing.IntValue();
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == State.Playing)
				{
					_tickTimer = CreateTickTimer();
				}
				return;
			}
		}

		private void ProcessSimulationCommandMessage(Message message)
		{
			var commandMessage = message as CommandMessage;
			if (commandMessage != null)
			{
				var command = commandMessage.Command;
				_resolver.ProcessCommand(command);
				return;
			}

			throw new Exception($"Unhandled Simulation Command Message: ${message}");
		}

		private void Tick()
		{
			//var commands = _commandSequence.Tick();
			//_resolver.ProcessCommands(commands);

			// TODO: reimplement tick!
			//_simulationRoot.Tick();

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
				_tickIntervalMS,
				_tickIntervalMS);
		}

		private void DestroyTimer(object timer)
		{
			PhotonPlugin.PluginHost.StopTimer(timer);
		}
	}
}
