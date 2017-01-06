using System;
using GameWork.Core.States;
namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class LobbyToGameTransition : EventStateTransition, IDisposable
	{
		private LobbyState _lobbyState;
		private bool _isDisposed;

		~LobbyToGameTransition()
		{
			Dispose();
		}

		public void Setup(LobbyState lobbyState)
		{
			_lobbyState = lobbyState;
			_lobbyState.GameStartedEvent += OnGameStarted;
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
