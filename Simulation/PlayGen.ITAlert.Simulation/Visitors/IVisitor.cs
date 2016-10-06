using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors
{
	public interface IVisitor : IITAlertEntity
	{
		INode CurrentNode { get; }

		void OnEnterNode(INode current);

		void OnExitNode(INode current);
	}
}
