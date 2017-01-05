using PlayGen.Photon.Players;
using System.Collections.Generic;
using System.Linq;

namespace PlayGen.ITAlert.Photon.Players.Extensions
{
	public static class StateExtensions
	{
		public static State GetCombinedStates(this IEnumerable<Player> players)
		{
			if (players.Any(p => p.State == (int)State.NotReady))
			{
				return State.NotReady;
			}
			else if (players.All(p => p.State == (int)State.Ready))
			{
				return State.Ready;
			}
			else if (players.All(p => p.State == (int)State.Initialized))
			{
				return State.Initialized;
			}
			else if (players.All(p => p.State == (int)State.Finalized))
			{
				return State.Finalized;
			}
			else if (players.All(p => p.State == (int)State.FeedbackSent))
			{
				return State.FeedbackSent;
			}

			return State.Error;
		}
	}
}