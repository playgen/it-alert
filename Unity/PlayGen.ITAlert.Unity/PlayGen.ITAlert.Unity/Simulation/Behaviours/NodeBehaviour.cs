using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public abstract class NodeBehaviour : EntityBehaviour
	{
		public VisitorVectors GetVisitorPosition(int pathPoint)
		{
			return GetPositionFromPathPoint(pathPoint);
		}

		protected abstract VisitorVectors GetPositionFromPathPoint(int pathPoint);
	}
}
