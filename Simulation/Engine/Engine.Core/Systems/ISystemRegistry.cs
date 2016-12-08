using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Systems
{
	public interface ISystemRegistry
	{
		void RegisterSystem(ISystem system);

		void Tick(int currentTick);
	}
}
