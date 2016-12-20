using System.Collections.Generic;
using System.Linq;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Events;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.ITAlert.Photon.Plugins.Extensions;
using PlayGen.ITAlert.TestData;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Commands.Interfaces;
using PlayGen.ITAlert.Simulation.Commands.Sequence;
using PlayGen.ITAlert.Photon.SUGAR;

namespace PlayGen.ITAlert.Photon.Plugins.RoomStates
{
	public class GameState : RoomState
	{
		public const string StateName = "Game";

		private Simulation.Simulation _simulation;
		private CommandSequence _commandSequence;
		private CommandResolver _resolver;
		private InternalGameState _internalState;
		private int _tickIntervalMS = 100;
		private object _tickTimer;

		public override string Name
		{
			get { return StateName; }
		}

		public GameState(PluginBase plugin, PlayerManager playerManager, Controller sugarController) 
            : base(plugin, playerManager, sugarController)
		{
		}

		public override void Initialize()
		{
			Plugin.PluginHost.TryRegisterType(typeof(Simulation.Simulation),
				SerializableTypes.SimulationState,
				Serializer.SerializeSimulation,
				Serializer.DeserializeSimulation);
		}

		#region Events
		public override void OnCreate(ICreateGameCallInfo info)
		{
		}

		public override void OnJoin(IJoinGameCallInfo info)
		{
		}

		public override void OnLeave(ILeaveGameCallInfo info)
		{
		}
		
		public override void OnRaiseEvent(IRaiseEventCallInfo info)
		{
			switch (info.Request.EvCode)
			{
				case (byte)PlayerEventCode.GameInitialized:
					PlayerManager.ChangeStatus(info.ActorNr, PlayerStatus.GameInitialized);

					if (PlayerManager.CombinedPlayerStatus == PlayerStatus.GameInitialized)
					{
						ChangeInternalState(InternalGameState.Playing);
					}
					break;

				case (byte)PlayerEventCode.GameCommand:
					var command = Serializer.Deserialize<ICommand>((byte[]) info.Request.Data);
					_resolver.ProcessCommand(command);
					break;

				case (byte)PlayerEventCode.GameFinalized:
					PlayerManager.ChangeStatus(info.ActorNr, PlayerStatus.GameFinalized);

					if (PlayerManager.CombinedPlayerStatus == PlayerStatus.GameFinalized)
					{
						ChangeState(LobbyState.StateName);   
					}
					break;
			}
		}
		#endregion

		public override void Enter()
		{
			Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId, (byte)ServerEventCode.GameEntered);

			List<int> subsystemLogicalIds;
			_simulation = InitializeSimulation(out subsystemLogicalIds);
			_commandSequence = CommandSequenceHelper.GenerateCommandSequence(subsystemLogicalIds, 100, 500, 2100);  // todo make values data driven - possibly via difficulty value set by players
			_resolver = new CommandResolver(_simulation);
			
			ChangeInternalState(InternalGameState.Initializing);

		    SugarController.StartMatch();
		}

		public override void Exit()
		{
            SugarController.EndMatch();

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

					_simulation.Tick();

					if (_simulation.IsGameFailure)
					{
						ChangeInternalState(InternalGameState.Finalizing);
					}
					else if(!_simulation.HasViruses && !_commandSequence.HasPendingCommands)
					{
						ChangeInternalState(InternalGameState.Finalizing);
					}
					else
					{
						BroadcastSimulation(ServerEventCode.GameTick, _simulation);
					}
					
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
			Plugin.BroadcastAll(RoomControllerPlugin.ServerPlayerId,
				(byte) eventCode,
				_simulation);
		}
		
		private object CreateTickTimer()
		{
			return Plugin.PluginHost.CreateTimer(
				Tick,
				_tickIntervalMS,
				_tickIntervalMS);
		}

		private void DestroyTimer(object timer)
		{
			Plugin.PluginHost.StopTimer(timer);
		}

		private Simulation.Simulation InitializeSimulation(out List<int> subsystemLogicalIds)
		{
			var players = Plugin.PluginHost.GameActorsActive.Select(p =>
			{
				var player = PlayerManager.Get(p.ActorNr);

				return new PlayerConfig
				{
					ExternalId = player.PhotonId,
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