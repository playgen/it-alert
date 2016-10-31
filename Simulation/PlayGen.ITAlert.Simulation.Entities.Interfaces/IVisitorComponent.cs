namespace PlayGen.ITAlert.Simulation.Entities.Interfaces
{
	public interface IVisitorComponent : IComponent
	{
		void OnEnterNode(INode current);

		void OnExitNode(INode current);

		void OnTick(INode currentNode);
	}
}
