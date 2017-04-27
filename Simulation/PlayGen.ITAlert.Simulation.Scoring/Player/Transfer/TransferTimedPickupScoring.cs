using System;
using System.Collections.Generic;
using Engine.Components;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Scoring;
using PlayGen.ITAlert.Simulation.Modules.Transfer.Events;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.Transfer
{
	public class TransferTimedPickupScoring : IPlayerScoringExtension
	{
		private readonly IDisposable _transferActivationSubscription;
		private readonly IDisposable _pickupItemSubscription;
		private readonly IDisposable _activateItemSubscription;

		private readonly Dictionary<int, TransferActivationEvent> _transferTiming;

		private const int ScoreWindowSeconds = 4;

		private readonly int TickWindow = (1000 * ScoreWindowSeconds) / SimulationConstants.TickInterval;

		private readonly ComponentMatcherGroup<ITAlert.Simulation.Components.EntityTypes.Player, Score> _playerScoreMatcherGroup;

		public TransferTimedPickupScoring(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_transferActivationSubscription = eventSystem.Subscribe<TransferActivationEvent>(OnTransferActivationEvent);
			_pickupItemSubscription = eventSystem.Subscribe<PickupItemEvent>(OnPickupItemEvent);
			_activateItemSubscription = eventSystem.Subscribe<ActivateItemEvent>(OnActivateItemEvent);

			_transferTiming = new Dictionary<int, TransferActivationEvent>();

			_playerScoreMatcherGroup = matcherProvider.CreateMatcherGroup<ITAlert.Simulation.Components.EntityTypes.Player, Score>();
		}

		private void OnTransferActivationEvent(TransferActivationEvent transferActivationEvent)
		{
			var addLocal = false;
			var addRemote = false;

			switch (transferActivationEvent.ActivationResult)
			{
				case TransferActivationEvent.TransferActivationResult.PulledItem:
					addRemote = true;
					break;
				case TransferActivationEvent.TransferActivationResult.PushedItem:
					addLocal = true;
					break;
				case TransferActivationEvent.TransferActivationResult.SwappedItems:
					addRemote = true;
					addLocal = true;
					break;
			}

			if (addLocal && transferActivationEvent.LocalItemEntityId.HasValue)
			{
				_transferTiming[transferActivationEvent.LocalItemEntityId.Value] = transferActivationEvent;
			}

			if (addRemote && transferActivationEvent.RemoteItemEntityId.HasValue)
			{
				_transferTiming[transferActivationEvent.RemoteItemEntityId.Value] = transferActivationEvent;
			}
		}
		
		private void OnPickupItemEvent(PickupItemEvent pickupItemEvent)
		{
			if (pickupItemEvent.Result == PickupItemEvent.ActivationResult.Success
				&& _transferTiming.TryGetValue(pickupItemEvent.ItemId, out var transferEvent))
			{
				if (pickupItemEvent.Tick - transferEvent.Tick > TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(pickupItemEvent.ItemId);
			}
		}

		private void OnActivateItemEvent(ActivateItemEvent activateItemEvent)
		{
			if (activateItemEvent.Result == ActivateItemEvent.ActivationResult.Success
				&& _transferTiming.TryGetValue(activateItemEvent.ItemId, out var transferEvent))
			{
				if (activateItemEvent.Tick - transferEvent.Tick > TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(activateItemEvent.ItemId);
			}
		}

		public void Dispose()
		{
			_transferActivationSubscription?.Dispose();
			_pickupItemSubscription?.Dispose();
			_activateItemSubscription?.Dispose();
			_playerScoreMatcherGroup?.Dispose();
		}
	}
}
