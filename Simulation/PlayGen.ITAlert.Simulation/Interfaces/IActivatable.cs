namespace PlayGen.ITAlert.Simulation.Interfaces
{
	public interface IActivatable
	{
		bool IsActive { get; }

		void Activate();

		void Deactivate();
	}
}
