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
		private readonly IDisposable _pickupItemAndActivateHeldSubscription;
		private readonly IDisposable _pickupItemAndSwapWithHeldSubscription;
		private readonly IDisposable _moveItemSubscription;
		private readonly IDisposable _swapItemSubscription;

		private readonly Dictionary<int, TransferActivationEvent> _transferTiming;

		private const int ScoreWindowSeconds = 9;

		private readonly int TickWindow = (1000 * ScoreWindowSeconds) / SimulationConstants.TickInterval;

		private readonly ComponentMatcherGroup<ITAlert.Simulation.Components.EntityTypes.Player, Score> _playerScoreMatcherGroup;

		public TransferTimedPickupScoring(IMatcherProvider matcherProvider, EventSystem eventSystem)
		{
			_transferActivationSubscription = eventSystem.Subscribe<TransferActivationEvent>(OnTransferActivationEvent);
			_pickupItemSubscription = eventSystem.Subscribe<PickupItemEvent>(OnPickupItemEvent);
			_activateItemSubscription = eventSystem.Subscribe<ActivateItemEvent>(OnActivateItemEvent);
			_pickupItemAndActivateHeldSubscription = eventSystem.Subscribe<SwapInventoryItemAndActivateEvent>(OnPickUpAndActivateHeldEvent);
			_pickupItemAndSwapWithHeldSubscription = eventSystem.Subscribe<SwapInventoryItemCommandEvent>(OnPickUpAndSwapHeldEvent);
			_moveItemSubscription = eventSystem.Subscribe<MoveItemCommandEvent>(OnMoveItemEvent);
			_swapItemSubscription = eventSystem.Subscribe<SwapSubsystemItemCommandEvent>(OnSwapItemEvent);

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
				if (pickupItemEvent.Tick - transferEvent.Tick <= TickWindow
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
				if (activateItemEvent.Tick - transferEvent.Tick <= TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(activateItemEvent.ItemId);
			}
		}

		private void OnPickUpAndActivateHeldEvent(SwapInventoryItemAndActivateEvent pickUpItemEvent)
		{
			if (pickUpItemEvent.Result == SwapInventoryItemAndActivateEvent.ActivationResult.Success
				&& _transferTiming.TryGetValue(pickUpItemEvent.SubsystemItemId, out var transferEvent))
			{
				if (pickUpItemEvent.Tick - transferEvent.Tick <= TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(pickUpItemEvent.SubsystemItemId);
			}
		}

		private void OnPickUpAndSwapHeldEvent(SwapInventoryItemCommandEvent pickUpItemEvent)
		{
			if (pickUpItemEvent.Result == SwapInventoryItemCommandEvent.ActivationResult.Success
				&& _transferTiming.TryGetValue(pickUpItemEvent.SubsystemItemId, out var transferEvent))
			{
				if (pickUpItemEvent.Tick - transferEvent.Tick <= TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(pickUpItemEvent.SubsystemItemId);
			}
		}

		private void OnMoveItemEvent(MoveItemCommandEvent moveItemEvent)
		{
			if (moveItemEvent.Result == MoveItemCommandEvent.ActivationResult.Success
				&& _transferTiming.TryGetValue(moveItemEvent.ItemId, out var transferEvent))
			{
				if (moveItemEvent.Tick - transferEvent.Tick <= TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(moveItemEvent.ItemId);
			}
		}

		private void OnSwapItemEvent(SwapSubsystemItemCommandEvent moveItemEvent)
		{
			if (moveItemEvent.Result == SwapSubsystemItemCommandEvent.ActivationResult.Success
				&& (_transferTiming.TryGetValue(moveItemEvent.FromItemId, out var transferEvent) || _transferTiming.TryGetValue(moveItemEvent.ToItemId, out transferEvent)))
			{
				if (moveItemEvent.Tick - transferEvent.Tick <= TickWindow
					&& _playerScoreMatcherGroup.TryGetMatchingEntity(transferEvent.PlayerEntityId, out var playerTuple))
				{
					playerTuple.Component2.ResourceManagement += 1;
				}
				_transferTiming.Remove(_transferTiming.TryGetValue(moveItemEvent.FromItemId, out var _) ? moveItemEvent.FromItemId : moveItemEvent.ToItemId);
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
