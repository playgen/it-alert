namespace PlayGen.ITAlert.Simulation.Entities
{
	public interface IActivatable
	{
		bool IsActive { get; }

		void Activate();

		void Deactivate();
	}
}
