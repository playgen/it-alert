using System;
using GameWork.Core.States.Event;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class LobbyToGameTransition : EventStateTransition, IDisposable
	{
		private readonly LobbyState _lobbyState;
		private bool _isDisposed;

		public LobbyToGameTransition(LobbyState lobbyState)
		{
			_lobbyState = lobbyState;
			_lobbyState.GameStartedEvent += OnGameStarted;
		}

		~LobbyToGameTransition()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_lobbyState.GameStartedEvent -= OnGameStarted;

			_isDisposed = true;
		}

		private void OnGameStarted()
		{
			ChangeState(GameState.StateName);
		}
	}
}
