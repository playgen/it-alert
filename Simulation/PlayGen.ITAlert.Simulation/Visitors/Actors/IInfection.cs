namespace PlayGen.ITAlert.Simulation.Visitors.Actors
{
	public interface IInfection : IVisitor
	{
		bool Visible { get; }

		void SetVisible(bool visible);
	}
}
