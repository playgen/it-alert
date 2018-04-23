using System;

namespace PlayGen.ITAlert.Simulation.Scoring.Player.GameActions
{
	public class GameAction
	{
		public string Identifier;
		public bool Success;
		public int Count;

		public GameAction(string identifier, bool success = true)
		{
			Identifier = identifier;
			Success = success;
		}
	}
}
