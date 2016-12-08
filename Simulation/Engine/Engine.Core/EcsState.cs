using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine
{
	public class EcsState
	{
		public Dictionary<int, StateBucket> EntityStates { get;  }

		public EcsState(Dictionary<int, StateBucket> entityStates)
		{
			EntityStates = entityStates;
		}
	}
}
