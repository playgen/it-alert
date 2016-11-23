using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Systems;

namespace PlayGen.ITAlert.Simulation.VisitorsProperty.Actors.Npc
{
	public class EmitConsumeMemoryBehviourBehaviourComponent : VisitorBehaviourComponent
	{ 
		public EmitConsumeMemoryBehviourBehaviourComponent(IEntity entity) 
			: base(entity)
		{
		}

		public override void OnEnterNode(INode current)
		{
			base.OnEnterNode(current);


		}
	}
}

