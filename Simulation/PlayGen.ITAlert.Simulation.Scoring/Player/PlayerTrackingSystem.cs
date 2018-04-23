using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Configuration;
using Engine.Events;
using Engine.Lifecycle.Events;
using Engine.Systems;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Components.Scoring;
using PlayGen.ITAlert.Simulation.Modules.Antivirus.Events;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;
using PlayGen.ITAlert.Simulation.Scoring.Player.GameActions;
using PlayGen.ITAlert.Simulation.Systems.Movement;
using Zenject;

namespace PlayGen.ITAlert.Simulation.Scoring.Player
{
	public class PlayerTrackingSystem : ITickableSystem
	{
		private List<IPlayerTrackingExtension> _extensions;
		private readonly EventSystem _eventSystem;

		private readonly IDisposable _transferActivationSubscription;
		private readonly IDisposable _activateItemSubscription;
		private readonly IDisposable _antivirusUseSubscription;
		private readonly IDisposable _moveToSubsystemSubscription;

		private readonly List<PlayerGameActions> _playerGameActions = new List<PlayerGameActions>();

		public string GetActions()
		{
			var str = "";
			foreach (var playerGameAction in _playerGameActions)
			{
				str += "Player: " + playerGameAction.PlayerEntityId + "\n";
				foreach (var gameAction in playerGameAction.GameActions)
				{
					str += gameAction.Identifier + ": " + gameAction.Count + "\n";
				}
				str += "\n";
			}
			return str;
		}

		public PlayerTrackingSystem([InjectOptional] List<IPlayerTrackingExtension> extensions, EventSystem eventSystem)
		{
			_extensions = extensions;
			_eventSystem = eventSystem;

			_transferActivationSubscription = eventSystem.Subscribe<TransferActivationEvent>(OnTransferActivationEvent);
			_activateItemSubscription = eventSystem.Subscribe<ActivateItemEvent>(OnActivateItemEvent);
			_antivirusUseSubscription = eventSystem.Subscribe<AntivirusActivationEvent>(OnAntivirusUseEvent);
			_moveToSubsystemSubscription = eventSystem.Subscribe<PlayerLeaveNodeEvent>(OnSetDestinationEvent);
		}

		private void OnTransferActivationEvent(TransferActivationEvent transferActivationEvent)
		{
			if ((int)transferActivationEvent.ActivationResult >= 2)
			{
				AddAction(transferActivationEvent.PlayerEntityId, transferActivationEvent.GetType().Name);
			}
		}


		private void OnActivateItemEvent(ActivateItemEvent activateItemEvent)
		{
			if (activateItemEvent.Result == ActivateItemEvent.ActivationResult.Success)
			{
				AddAction(activateItemEvent.PlayerEntityId, activateItemEvent.ItemType);
			}
		}

		private void OnAntivirusUseEvent(AntivirusActivationEvent antivirusActivationEvent)
		{
			if (antivirusActivationEvent.ActivationResult == AntivirusActivationEvent.AntivirusActivationResult.SoloExtermination)
			{
				AddAction(antivirusActivationEvent.PlayerEntityId, "Exterminate");
			}
		}

		private void OnSetDestinationEvent(PlayerLeaveNodeEvent playerLeaveNodeEvent)
		{
			AddAction(playerLeaveNodeEvent.PlayerEntityId, "Move");
		}

		private void AddAction(int playerEntityId, string action)
		{
			var existing = _playerGameActions.FirstOrDefault(a => a.PlayerEntityId == playerEntityId);
			if (existing == null)
			{
				existing = new PlayerGameActions
				{
					PlayerEntityId = playerEntityId
				};
				_playerGameActions.Add(existing);
			}
			existing.CompletedAction(action);
		}

		private void RemoveAction(int playerEntityId, string action)
		{
			var existing = _playerGameActions.FirstOrDefault(a => a.PlayerEntityId == playerEntityId);
			existing?.CancelledAction(action);
		}


		public void Dispose()
		{
			_transferActivationSubscription?.Dispose();
			_activateItemSubscription?.Dispose();
			_moveToSubsystemSubscription?.Dispose();
		}

		public void Tick(int currentTick)
		{
			foreach (var extension in _extensions.OfType<Engine.ITickable>())
			{
				extension.Tick(currentTick);
			}
		}
	}
}
