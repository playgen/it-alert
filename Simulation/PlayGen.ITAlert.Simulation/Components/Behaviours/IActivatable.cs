using Engine.Components;

namespace PlayGen.ITAlert.Simulation.Components.Behaviours
{
	public interface IActivatable : IComponent
	{


		void OnActivating();

		void OnActive();

		void OnDeactivating();

	}
}
