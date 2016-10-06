﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Contracts.Extensions
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
