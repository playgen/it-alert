using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Entities
{
	public static class VisitorPositionExtensions
	{
		public static PositionState ToPositionState(this VisitorPosition visitorPosition, IITAlertEntity relativeEntity)
		{
			var relativeTick = visitorPosition.CurrentTick - relativeEntity.CurrentTick;
			return new PositionState()
			{
				Position = visitorPosition.Position,
				RelativeCycle = relativeTick == 0 ? (int?) null : relativeTick,
			};
		}
	}
}
