using PlayGen.Engine.Components;
using PlayGen.ITAlert.Simulation.Entities.World;

namespace PlayGen.ITAlert.Simulation.Entities.Visitors
{
	public interface IVisitorComponent : IComponent
	{
		void OnEnterNode(INode current);

		void OnExitNode(INode current);

		void OnTick(INode currentNode);
	}
}
