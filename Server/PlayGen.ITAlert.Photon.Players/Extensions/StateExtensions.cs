using PlayGen.Photon.Players;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Players.Extensions
{
	public static class StateExtensions
	{
		public static ClientState GetCombinedStates(this IEnumerable<Player> players)
		{
			var playerList = players as IList<Player> ?? players.ToList();
			if (playerList.Any(p => p.State == (int)ClientState.NotReady))
			{
				return ClientState.NotReady;
			}
			if (playerList.All(p => p.State == (int)ClientState.Ready))
			{
				return ClientState.Ready;
			}
			if (playerList.All(p => p.State == (int)ClientState.Initializing))
			{
				return ClientState.Initializing;
			}
			if (playerList.All(p => p.State == (int)ClientState.Initialized))
			{
				return ClientState.Initialized;
			}
			if (playerList.All(p => p.State == (int)ClientState.Playing))
			{
				return ClientState.Playing;
			}
			return playerList.All(p => p.State == (int)ClientState.FeedbackSent) ? ClientState.FeedbackSent : ClientState.Error;
		}
	}
}