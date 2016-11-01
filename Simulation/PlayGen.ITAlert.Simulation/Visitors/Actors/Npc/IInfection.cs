namespace PlayGen.ITAlert.Simulation.Visitors.Actors.Npc
{
	public interface IInfection : IVisitor
	{
		bool Visible { get; }

		void SetVisible(bool visible);
	}
}
