namespace PlayGen.Engine.Components.Behaviour
{
	public interface ITimedActivatableBehaviour : ITickableComponent
	{

		void OnActivationRequested();

		void OnDeactivationRequested();

	}
}
