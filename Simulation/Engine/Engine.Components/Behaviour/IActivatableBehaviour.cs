namespace PlayGen.Engine.Components.Behaviour
{
	public interface IActivatableBehaviour
	{
		bool CanActivate { get; }

		bool CanDeactivate { get; }

		bool IsActive { get; }


		void OnActivating();

		void OnActive();

		void OnDeactivating();

		void Activate();

		void Deactivate();
	}
}
