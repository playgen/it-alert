using GameWork.Core.States.Event;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Players;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Players.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class CombinedPlayersStateTransition : EventStateTransition
	{
		private readonly State _combinedState;
		private readonly string _toStateName;

		public CombinedPlayersStateTransition(State combinedPlayerState, string toStateName)
		{
			_combinedState = combinedPlayerState;
			_toStateName = toStateName;
		}

		public void OnPlayersStateChange(List<Player> players)
		{
			if (players.GetCombinedStates() == _combinedState)
			{
				ChangeState(_toStateName);
			}
		}
	}
}
