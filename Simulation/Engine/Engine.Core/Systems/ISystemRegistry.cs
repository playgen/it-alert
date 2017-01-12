using System.Collections.Generic;

namespace Engine.Systems
{
	public interface ISystemRegistry
	{
		TSystem GetSystem<TSystem>() where TSystem : class, ISystem;

		IList<TSystem> GetSystems<TSystem>() where TSystem : class, ISystem;

		void Initialize();
		void Tick(int currentTick);
	}
}