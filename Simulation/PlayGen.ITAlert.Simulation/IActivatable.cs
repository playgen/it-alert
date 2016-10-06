namespace PlayGen.ITAlert.Simulation
{
	public interface IActivatable
	{
		bool IsActive { get; }

		void Activate();

		void Deactivate();
	}
}
