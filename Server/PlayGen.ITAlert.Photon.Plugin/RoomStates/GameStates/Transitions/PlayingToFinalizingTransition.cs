using System;
using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class PlayingToFinalizingTransition : EventStateTransition, IDisposable
	{
		private readonly PlayingState _playingState;
		private bool _isDisposed;

		public PlayingToFinalizingTransition(PlayingState playingState)
		{
			_playingState = playingState;
			_playingState.GameOverEvent += OnGameOver;
		}

		~PlayingToFinalizingTransition()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_playingState.GameOverEvent -= OnGameOver;

			_isDisposed = true;
		}

		private void OnGameOver()
		{
			ChangeState(FinalizingState.StateName);
		}
	}
}
