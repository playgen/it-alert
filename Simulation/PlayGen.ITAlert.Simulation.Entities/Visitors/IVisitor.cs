using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Entities.Visitors
{
	public interface IVisitor : IITAlertEntity
	{
		INode CurrentNode { get; }

		void OnEnterNode(INode current);

		void OnExitNode(INode current);
	}
}
