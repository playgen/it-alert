using System;
using GameWork.Core.States;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class PlayingToFinalizingTransition : EventStateTransition, IDisposable
	{
		private PlayingState _playingState;
		private bool _isDisposed;

		~PlayingToFinalizingTransition()
		{
			Dispose();
		}

		public void Setup(PlayingState playingState)
		{
			_playingState = playingState;
			_playingState.GameOverEvent += OnGameOver;
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
