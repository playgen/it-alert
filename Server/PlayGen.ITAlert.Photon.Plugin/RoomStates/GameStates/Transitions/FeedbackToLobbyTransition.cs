using System;
using System.Collections.Generic;
using GameWork.Core.States.Event;
using PlayGen.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class FeedbackToLobbyTransition : EventStateTransition, IDisposable
	{
		private readonly FeedbackState _feedbackState;
		private bool _isDisposed;

		public FeedbackToLobbyTransition(FeedbackState feedbackState)
		{
			_feedbackState = feedbackState;
			_feedbackState.PlayerFeedbackSentEvent += OnPlayerFeedbackSent;
		}

		~FeedbackToLobbyTransition()
		{
			Dispose();
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
