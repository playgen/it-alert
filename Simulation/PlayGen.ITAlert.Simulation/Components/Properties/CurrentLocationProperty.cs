using Engine.Components.Property;
using Engine.Core.Entities;
using PlayGen.ITAlert.Simulation.Components.Messages;

namespace PlayGen.ITAlert.Simulation.Components.Properties
{
	public class CurrentLocationProperty : Property<int>
	{
		public CurrentLocationProperty(IEntity entity) 
			: base(entity)
		{

		}

		private void VisitorEnteredNode(VisitorEnteredNodeMessage visitorEnteredNodeMessage)
		{
			//if (visitorEnteredNodeMessage.Visitor.Equals())
			//SetValue(visitorEnteredNodeMessage.Node);
		}
	}
}
