using System.Collections.Generic;
using System.Linq;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.PhotonPlugins.Extensions;
using PlayGen.ITAlert.PhotonPlugins.RoomStates.Interfaces;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Interfaces;
using PlayGen.ITAlert.Simulation.Commands.Sequence;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.TestData;

namespace PlayGen.ITAlert.PhotonPlugins.RoomStates
{
	public class GameState : GameWork.Core.States.State, IRoomState
	{
		public const string StateName = "Game";

		private readonly PluginBase _plugin;
		private readonly PlayerManager _playerManager;

		private Simulation.Simulation _simulation;
		private CommandSequence _commandSequence;
		private CommandResolver _resolver;
		private InternalGameState _internalState;
		private int _tickIntervalMS = 100;
		private object _tickTimer;

		private ICommand _command = new RequestMovePlayerCommand();

		public override string Name
		{
			get { return StateName; }
		}

		public GameState(PluginBase plugin, PlayerManager playerManager)
		{
			_plugin = plugin;
			_playerManager = playerManager;
		}

		public override void Initialize()
		{
			_plugin.PluginHost.TryRegisterType(typeof(Simulation.Simulation),
				SerializableTypes.SimulationState,
				Serializer.SerializeSimulation,
				Serializer.DeserializeSimulation);
		}

		#region Events
		public void OnCreate(ICreateGameCallInfo info)
		{
		}

		public void OnJoin(IJoinGameCallInfo info)
		{
		}

		public void OnLeave(ILeaveGameCallInfo info)
		{
		}
		
		public void OnRaiseEvent(IRaiseEventCallInfo info)
		{
			switch (info.Request.EvCode)
			{
				case (byte)PlayerEventCode.GameInitialized:
					_playerManager.ChangeStatus(info.ActorNr, PlayerStatuses.GameInitialized);

					if (_playerManager.CombinedPlayerStatuses == PlayerStatuses.GameInitialized)
					{
						ChangeInternalState(InternalGameState.Playing);
					}
					break;

				case (byte)PlayerEventCode.GameCommand:
					var command = Serializer.Deserialize<ICommand>((byte[]) info.Request.Data);
					_resolver.ProcessCommand(command);
					break;

				case (byte)PlayerEventCode.GameFinalized:
					_playerManager.ChangeStatus(info.ActorNr, PlayerStatuses.GameFinalized);

					if (_playerManager.CombinedPlayerStatuses == PlayerStatuses.GameFinalized)
					{
						ChangeState(LobbyState.StateName);   
					}
					break;
			}
		}
		#endregion

		public override void Enter()
		{
			_plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.GameEntered);

			List<int> subsystemLogicalIds;
			_simulation = InitializeSimulation(out subsystemLogicalIds);
			_commandSequence = CommandSequenceHelper.GenerateCommandSequence(subsystemLogicalIds, 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
			_resolver = new CommandResolver(_simulation);
			
			ChangeInternalState(InternalGameState.Initializing);
		}

		public override void Exit()
		{
			_resolver = null;
			_simulation.Dispose();
			_simulation = null;
		}
		
		private void Tick()
		{          
			switch (_internalState)
			{
				case InternalGameState.Initializing:
					break;

				case InternalGameState.Playing:

					var commands = _commandSequence.Tick();
					_resolver.ProcessCommands(commands);

					//_simulation.Tick();

					//if (_simulation.IsGameFailure)
					//{
					//	ChangeInternalState(InternalGameState.Finalizing);
					//}
					//else if(!_simulation.HasViruses && !_commandSequence.HasPendingCommands)
					//{
					//	ChangeInternalState(InternalGameState.Finalizing);
					//}
					//else
					//{
						BroadcastSimulation(ServerEventCode.GameTick, _simulation);
					//}
					
					break;

				case InternalGameState.Finalizing:
					break;
			}
		}

		private void ChangeInternalState(InternalGameState toState)
		{
			switch (toState)
			{
				case InternalGameState.Initializing:
					BroadcastSimulation(ServerEventCode.GameInitialized, _simulation);
					break;
					
				case InternalGameState.Playing:
					_tickTimer = CreateTickTimer();
					break;

				case InternalGameState.Finalizing:
					DestroyTimer(_tickTimer);
					BroadcastSimulation(ServerEventCode.GameFinalized, _simulation);
					break;
			}

			_internalState = toState;
		}

		private void BroadcastSimulation(ServerEventCode eventCode, Simulation.Simulation simulation)
		{
			_plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId,
				(byte) eventCode,
				_simulation);
		}
		
		private object CreateTickTimer()
		{
			return _plugin.PluginHost.CreateTimer(
				Tick,
				_tickIntervalMS,
				_tickIntervalMS);
		}

		private void DestroyTimer(object timer)
		{
			_plugin.PluginHost.StopTimer(timer);
		}

		private Simulation.Simulation InitializeSimulation(out List<int> subsystemLogicalIds)
		{
			var players = _plugin.PluginHost.GameActorsActive.Select(p =>
			{
				var player = _playerManager.Get(p.ActorNr);

				return new PlayerConfig
				{
					ExternalId = player.Id,
					Name = player.Name,
					Colour = "#" + player.Color,
				};
			}).ToList();

			// todo make config data driven
			var simulation = ConfigHelper.GenerateSimulation(2, 2, players, 2, 4, out subsystemLogicalIds);
			return simulation;
		}
	}
}