using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.GameActions
{
	public class PlayerGameActions : Event, IPlayerEvent
	{
		public int PlayerEntityId { get; set; }

		public List<GameAction> GameActions = new List<GameAction>();

		public void CompletedAction(string identifier)
		{
			SetAction(identifier, 1);
		}
		public void CancelledAction(string identifier)
		{
			SetAction(identifier, -1);
		}

		private void SetAction(string identifier, int incremnt)
		{
			var action = GameActions.FirstOrDefault(a => a.Identifier == identifier);
			if (action == null)
			{
				action = new GameAction(identifier);
				GameActions.Add(action);
			}

			action.Count = Math.Max(0,
				action.Count + incremnt);
		}

		public int GetActionCount(string identifyer)
		{
			var action = GameActions.FirstOrDefault(a => a.Identifier == identifyer);
			return action?.Count ?? 0;
		}

		public int AntivirusUsedWrongly()
		{
			return GetActionCount("Antivirus") - GetActionCount("Virus");
		}
	}
}
