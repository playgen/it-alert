using Engine.Core.Serialization;
using PlayGen.ITAlert.Simulation.Visitors;

namespace PlayGen.ITAlert.Simulation.Systems.Behaviours
{
	public class VisitorPosition
	{
		[SyncState(StateLevel.Differential)]
		public IVisitor Visitor { get; }

		[SyncState(StateLevel.Differential)]
		public int CurrentTick { get; private set; }

		[SyncState(StateLevel.Differential)]
		public int Position { get; private set; }

		public VisitorPosition(IVisitor visitor, int position, int currentTick)
		{
			Visitor = visitor;
			Position = position;
			CurrentTick = currentTick;
		}

		/// <summary>
		/// Provide a tick safe method to update the current position
		/// </summary>
		/// <param name="position"></param>
		/// <param name="currentTick"></param>
		/// <returns>True if the position was updated. False if this position has been updated already this tick.</returns>
		public bool UpdatePosition(int position, int currentTick)
		{
			if (currentTick > CurrentTick)
			{
				Position = position;
				CurrentTick = currentTick;
				return true;
			}
			return false;
		}
	}
}
