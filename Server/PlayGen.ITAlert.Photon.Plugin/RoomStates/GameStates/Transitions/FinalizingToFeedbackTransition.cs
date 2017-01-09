using System;
using System.Collections.Generic;
using GameWork.Core.States.Event;
using PlayGen.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class FinalizingToFeedbackTransition : EventStateTransition, IDisposable
	{
		private readonly FinalizingState _finalizingState;
		private bool _isDisposed;

		public FinalizingToFeedbackTransition(FinalizingState finalizingState)
		{
			_finalizingState = finalizingState;
			_finalizingState.PlayerFinalizedEvent += OnPlayerFinalized;
		}

		~FinalizingToFeedbackTransition()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_finalizingState.PlayerFinalizedEvent -= OnPlayerFinalized;

			_isDisposed = true;
		}

		private void OnPlayerFinalized(List<Player> players)
		{
			if (players.GetCombinedStates() == State.Finalized)
			{
				ChangeState(FeedbackState.StateName);
			}
		}
	}
}
