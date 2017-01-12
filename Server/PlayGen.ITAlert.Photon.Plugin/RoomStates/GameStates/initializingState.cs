using System;
using System.Collections.Generic;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Serialization;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Plugin.States;
using PlayGen.Photon.SUGAR;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Game.States;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates
{
	public class InitializingState : RoomState
	{
		public const string StateName = "Initializing";

		private readonly Simulation.Simulation _simulation;
	
		public override string Name => StateName;

		public event Action<List<Player>> PlayerInitializedEvent;

		public InitializingState(Simulation.Simulation simulation, PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController) 
			: base(photonPlugin, messenger, playerManager, sugarController)
		{
			_simulation = simulation;
		}

		protected override void OnEnter(string fromStateName)
		{
			Messenger.Subscribe((int)Channels.GameState, ProcessGameStateMessage);	

			Messenger.SendAllMessage(new InitializingMessage());
		}

		protected override void OnExit(string toStateName)
		{
			Messenger.Unsubscribe((int)Channels.GameState, ProcessGameStateMessage);
		}

		private void ProcessGameStateMessage(Message message)
		{
			var initializingMessage = message as InitializingMessage;
			if (initializingMessage != null)
			{
				var player = PlayerManager.Get(initializingMessage.PlayerPhotonId);
				player.State = (int)State.Initializing;
				PlayerManager.UpdatePlayer(player);

				if (PlayerManager.Players.GetCombinedStates() == State.Initializing)
				{
					Messenger.SendAllMessage(new Messages.Simulation.States.InitializedMessage
					{
						SerializedSimulation = Serializer.SerializeSimulation(_simulation)
					});
				}
				return;
			}

			var initializedMessage = message as Messages.Game.States.InitializedMessage;
			if (initializedMessage != null)
			{
				var player = PlayerManager.Get(initializedMessage.PlayerPhotonId);
				player.State = (int)State.Initialized;
				PlayerManager.UpdatePlayer(player);

				PlayerInitializedEvent(PlayerManager.Players);
				return;
			}
		}
	}
}
