using System;
using System.Collections.Generic;
using GameWork.Core.States;
using PlayGen.Photon.Players;
using PlayGen.ITAlert.Photon.Players.Extensions;
using State = PlayGen.ITAlert.Photon.Players.State;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.GameStates.Transitions
{
	public class InitializingToPlayingTransition : EventStateTransition, IDisposable
	{
		private InitializingState _initializingState;
		private bool _isDisposed;

		~InitializingToPlayingTransition()
		{
			Dispose();
		}

		public void Setup(InitializingState initializingState)
		{
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
				ChangeState(PlayingState.StateName);
			}
		}
	}
}
