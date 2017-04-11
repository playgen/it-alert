using PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions
{
	public class HideText : SimulationAction
	{
		public HideText()
		{
			Action = (ecs, configuration) => {
				ecs.EnqueueCommand(new HideTextCommand());
			};
		}
	}
}
