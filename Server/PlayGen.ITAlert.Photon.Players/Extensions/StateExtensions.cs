using PlayGen.Photon.Players;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Players.Extensions
{
	public static class StateExtensions
	{
		public static ClientState GetCombinedStates(this IEnumerable<Player> players)
		{
			if (players.Any(p => p.State == (int)ClientState.NotReady))
			{
				return ClientState.NotReady;
			}
			else if (players.All(p => p.State == (int)ClientState.Ready))
			{
				return ClientState.Ready;
			}
			else if (players.All(p => p.State == (int)ClientState.Initializing))
			{
				return ClientState.Initializing;
			}
			else if (players.All(p => p.State == (int)ClientState.Initialized))
			{
				return ClientState.Initialized;
			}
			else if (players.All(p => p.State == (int)ClientState.Playing))
			{
				return ClientState.Playing;
			}
			else if (players.All(p => p.State == (int)ClientState.FeedbackSent))
			{
				return ClientState.FeedbackSent;
			}

			return ClientState.Error;
		}
	}
}