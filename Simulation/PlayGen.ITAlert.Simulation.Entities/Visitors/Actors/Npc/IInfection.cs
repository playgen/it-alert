namespace PlayGen.ITAlert.Simulation.Entities.Visitors.Actors.Npc
{
	public interface IInfection : IVisitor
	{
		bool Visible { get; }

		void SetVisible(bool visible);
	}
}
