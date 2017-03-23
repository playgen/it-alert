using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Systems;

namespace PlayGen.ITAlert.Simulation.Systems.Npc
{
	public class NpcSystem : ISystem, ITickableSystem, IInitializingSystem
	{

		public NpcSystem()
		{
			
		}


		#region Implementation of IInitializingSystem

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		#endregion


		#region Implementation of ITickableSystem

		public void Tick(int currentTick)
		{
			throw new NotImplementedException();
		}


		#endregion
	}
}
