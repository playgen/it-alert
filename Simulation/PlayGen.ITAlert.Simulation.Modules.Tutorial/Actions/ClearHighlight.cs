using PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions
{
	public class ClearHighlight : SimulationAction
	{
		public ClearHighlight()
		{
			Action = (ecs, configuration) => {
				ecs.EnqueueCommand(new ClearHighlightCommand());
			};
		}
	}
}
