using System;
using GameWork.Core.States.Commands.Interfaces;
using GameWork.Core.States.Interfaces;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class LobbyToGameTransition : IStateTransition, IDisposable
	{
		private LobbyState _lobbyState;
		private IChangeStateAction _changeStateAction;
		private bool _isDisposed;

		~LobbyToGameTransition()
		{
			Dispose();
		}

		public void Setup(LobbyState lobbyState, IChangeStateAction changeStateAction)
		{
			_changeStateAction = changeStateAction;
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
			_changeStateAction.ChangeState(GameState.StateName);
		}
	}
}
