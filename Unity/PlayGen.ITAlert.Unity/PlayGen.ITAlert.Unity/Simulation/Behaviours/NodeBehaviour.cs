using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public abstract class NodeBehaviour : EntityBehaviour
	{
		public Vector3 GetVisitorPosition(int pathPoint)
		{
			var position = GetPositionFromPathPoint(pathPoint);
			return position;
		}

		protected abstract Vector3 GetPositionFromPathPoint(int pathPoint);
	}
}
