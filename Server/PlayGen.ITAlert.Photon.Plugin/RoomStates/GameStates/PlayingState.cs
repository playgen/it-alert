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
using PlayGen.ITAlert.TestData;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Messages.Simulation.Commands;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class PlayingState : RoomState
	{
		public const string StateName = "Playing";

		private readonly Simulation.Simulation _simulation;
		private readonly List<int> _subsystemLogicalIds;

		private CommandSequence _commandSequence;
		private CommandResolver _resolver;
		private int _tickIntervalMS = 100;
		private object _tickTimer;
		
		public override string Name => StateName;

		public event Action GameOverEvent;

		public PlayingState(List<int> subsystemLogicalIds, Simulation.Simulation simulation, PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController) 
			: base(photonPlugin, messenger, playerManager, sugarController)
		{
			_subsystemLogicalIds = subsystemLogicalIds;
			_simulation = simulation;
		}

		protected override void OnEnter()
		{
			Messenger.Subscribe((int)Channels.GameState, ProcessGameStateMessage);
			Messenger.Subscribe((int)Channels.SimulationCommand, ProcessSimulationCommandMessage);

			_commandSequence = CommandSequenceHelper.GenerateCommandSequence(_subsystemLogicalIds, 20, 20, 40);// todo uncomment: 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
			_resolver = new CommandResolver(_simulation);

			Messenger.SendAllMessage(new PlayingMessage());
		}

		protected override void OnExit()
		{
			Messenger.Unsubscribe((int)Channels.SimulationCommand, ProcessSimulationCommandMessage);
			Messenger.Unsubscribe((int)Channels.GameState, ProcessGameStateMessage);

			DestroyTimer(_tickTimer);
			_resolver = null;
			_commandSequence = null;
		}

		private void ProcessGameStateMessage(Message message)
		{
			var playingMessage = message as PlayingMessage;
			if (playingMessage != null)
			{
				var player = PlayerManager.Get(playingMessage.PlayerPhotonId);
				player.State = (int)State.Playing;
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == State.Playing)
				{
					Messenger.SendAllMessage(new InitializedMessage
					{
						SerializedSimulation = Serializer.SerializeSimulation(_simulation)
					});
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
			var commands = _commandSequence.Tick();
			_resolver.ProcessCommands(commands);

			_simulation.Tick();

			if (_simulation.IsGameFailure 
				|| (!_simulation.HasViruses && !_commandSequence.HasPendingCommands))
			{
				GameOverEvent();
			}
			else
			{
				Messenger.SendAllMessage(new TickMessage
				{
					SerializedSimulation = Serializer.SerializeSimulation(_simulation)
				});
			}
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
