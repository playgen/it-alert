using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public abstract class NodeBehaviour : EntityBehaviour
	{
		public Vector2 GetVisitorPosition(int pathPoint)
		{
			return GetPositionFromPathPoint(pathPoint);
		}

		protected abstract Vector2 GetPositionFromPathPoint(int pathPoint);
	}
}
