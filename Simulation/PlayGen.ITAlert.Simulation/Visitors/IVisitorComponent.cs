using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.Engine.Components;
using PlayGen.ITAlert.Simulation.World;

namespace PlayGen.ITAlert.Simulation.Visitors
{
	public interface IVisitorComponent : IComponent
	{
		void OnEnterNode(INode current);

		void OnExitNode(INode current);

		void OnTick(INode currentNode);
	}
}
