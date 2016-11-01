using PlayGen.ITAlert.Simulation.Contracts;
using PlayGen.ITAlert.Simulation.Systems.Behaviours;

namespace PlayGen.ITAlert.Simulation
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
