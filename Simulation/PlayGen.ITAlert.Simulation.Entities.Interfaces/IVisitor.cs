namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
{
	public interface IVisitor : IITAlertEntity
	{
		INode CurrentNode { get; }

		void OnEnterNode(INode current);

		void OnExitNode(INode current);
	}
}
