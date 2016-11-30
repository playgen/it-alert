using Engine.Components.Property;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Components.Messages;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class CurrentLocationProperty : Property<int>
	{
		private void VisitorEnteredNode(VisitorEnteredNodeMessage visitorEnteredNodeMessage)
		{
			//if (visitorEnteredNodeMessage.Visitor.Equals())
			//SetValue(visitorEnteredNodeMessage.Node);
		}
	}
}
