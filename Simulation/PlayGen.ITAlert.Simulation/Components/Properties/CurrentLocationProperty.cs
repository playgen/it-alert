using Engine.Components;
using Engine.Components.Property;
using Engine.Core.Components;
using PlayGen.ITAlert.Simulation.Systems;
using PlayGen.ITAlert.Simulation.Systems.Messages;

namespace PlayGen.ITAlert.Simulation.VisitorsProperty.Properties
{
	[ComponentUsage(typeof(IVisitor))]
	public class CurrentLocationProperty : Property<INode>
	{
		public CurrentLocationProperty(IEntity entity) 
			: base(container, "CurrentLocation", true)
		{
			Observer.AddSubscription<VisitorEnteredNodeMessage>(VisitorEnteredNode);

		}

		private void VisitorEnteredNode(VisitorEnteredNodeMessage visitorEnteredNodeMessage)
		{
			if (visitorEnteredNodeMessage.Visitor.Equals())
			Set(visitorEnteredNodeMessage.Node);
		}
	}
}
