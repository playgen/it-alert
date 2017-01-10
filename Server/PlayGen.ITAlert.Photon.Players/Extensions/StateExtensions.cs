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
			else if (players.All(p => p.State == (int)State.Playing))
			{
				return State.Playing;
			}
			else if (players.All(p => p.State == (int)State.FeedbackSent))
			{
				return State.FeedbackSent;
			}

			return State.Error;
		}
	}
}