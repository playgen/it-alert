using Engine.Core.Components;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.Visitors
{
	public interface IVisitorComponent : IComponent
	{
		void OnEnterNode(INode current);

		void OnExitNode(INode current);

		void OnTick(INode currentNode);
	}
}
