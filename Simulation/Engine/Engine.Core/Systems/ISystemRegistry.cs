namespace Engine.Systems
{
	public interface ISystemRegistry
	{
		TSystem GetSystem<TSystem>() where TSystem : class, ISystem;
		void Initialize();
		void RegisterSystem(ISystem system);
		void Tick(int currentTick);
	}
}