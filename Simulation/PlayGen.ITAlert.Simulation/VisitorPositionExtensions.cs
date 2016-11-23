using PlayGen.ITAlert.Simulation.Components.Behaviours;
using PlayGen.ITAlert.Simulation.Contracts;

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
