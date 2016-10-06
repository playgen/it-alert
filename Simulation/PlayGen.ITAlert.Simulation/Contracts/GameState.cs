using System.Collections.Generic;
using PlayGen.Engine;
using PlayGen.ITAlert.Simulation.Layout;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	/// <summary>
	/// Game State
	/// Queriable interface to return the current state of the simulation at the current tick.
	/// </summary>
	public class GameState
	{
		public Dictionary<int, ITAlertEntityState> Entities { get; set; }

		public Vector GraphSize { get; set; }

		public int CurrentTick { get; set; }

		public int Score { get; set; }

		public bool IsGameFailure { get; set; }
	}
}
