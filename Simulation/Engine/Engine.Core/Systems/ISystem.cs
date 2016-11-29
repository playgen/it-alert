using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Systems
{
	public interface ISystem
	{
		void Tick(int currentTick);

	}
}
