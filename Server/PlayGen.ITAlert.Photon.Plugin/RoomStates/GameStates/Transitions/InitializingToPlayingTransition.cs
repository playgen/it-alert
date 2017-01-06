using System;
using System.Collections.Generic;
using GameWork.Core.States.Commands.Interfaces;
using GameWork.Core.States.Interfaces;
using PlayGen.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using PlayGen.ITAlert.Photon.Players;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class InitializingToPlayingTransition : IStateTransition, IDisposable
	{
		private InitializingState _initializingState;
		private IChangeStateAction _changeStateAction;
		private bool _isDisposed;

		~InitializingToPlayingTransition()
		{
			Dispose();
		}

		public void Setup(InitializingState initializingState, IChangeStateAction changeStateAction)
		{
			_changeStateAction = changeStateAction;
			_initializingState = initializingState;
			_initializingState.PlayerInitializedEvent += OnPlayerInitialized;
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_initializingState.PlayerInitializedEvent -= OnPlayerInitialized;

			_isDisposed = true;
		}

		private void OnPlayerInitialized(List<Player> players)
		{
			if (players.GetCombinedStates() == State.Initialized)
			{
				_changeStateAction.ChangeState(PlayingState.StateName);
			}
		}
	}
}
