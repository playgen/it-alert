using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Commands.Tutorial;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Scenario.Actions
{
	public class ShowText : SimulationAction
	{
		public ShowText(bool @continue, params string[] text)
		{
			Action = (ecs, config) =>
			{
				var textCommand = new DisplayTextCommand()
				{
					Text = text.Aggregate(new StringBuilder(), (sb, t) => sb.AppendLine(t), sb => sb.ToString()),
					Continue = @continue,
				};
				ecs.EnqueueCommand(textCommand);
			};
		}
	}
}
