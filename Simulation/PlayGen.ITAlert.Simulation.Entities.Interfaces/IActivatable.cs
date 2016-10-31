namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
{
	public interface IActivatable
	{
		bool IsActive { get; }

		void Activate();

		void Deactivate();
	}
}
