using GameWork.Core.States.Event;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Players;
using System.Collections.Generic;
using PlayGen.ITAlert.Photon.Players.Extensions;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates.Transitions
{
	public class CombinedPlayersStateTransition : EventStateTransition
	{
		private readonly ClientState _combinedClientState;
		private readonly string _toStateName;

		public CombinedPlayersStateTransition(ClientState combinedPlayerClientState, string toStateName)
		{
			_combinedClientState = combinedPlayerClientState;
			_toStateName = toStateName;
		}

		public void OnPlayersStateChange(List<ITAlertPlayer> players)
		{
			if (players.GetCombinedStates() == _combinedClientState)
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
