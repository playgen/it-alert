using System;
using System.Collections.Generic;
using GameWork.Core.States;
using PlayGen.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class FeedbackToLobbyTransition : EventStateTransition, IDisposable
	{
		private FeedbackState _feedbackState;
		private bool _isDisposed;

		~FeedbackToLobbyTransition()
		{
			Dispose();
		}

		public void Setup(FeedbackState feedbackState)
		{
			_feedbackState = feedbackState;
			_feedbackState.PlayerFeedbackSentEvent += OnPlayerFeedbackSent;
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_feedbackState.PlayerFeedbackSentEvent -= OnPlayerFeedbackSent;

			_isDisposed = true;
		}

		private void OnPlayerFeedbackSent(List<Player> players)
		{
			if (players.GetCombinedStates() == State.FeedbackSent)
			{
				ChangeState(LobbyState.StateName);
			}
		}
	}
}
