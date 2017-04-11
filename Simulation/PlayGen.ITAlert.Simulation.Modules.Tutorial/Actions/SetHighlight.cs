using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands;
using PlayGen.ITAlert.Simulation.Sequencing;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Actions
{
	public class SetHighlight : SimulationAction
	{
		private readonly EntityConfig _entityConfig;

		public SetHighlight(EntityConfig entityConfig, bool enabled = true)
		{
			_entityConfig = entityConfig;
			Action = (ecs, configuration) => {
				ecs.EnqueueCommand(new SetHighlightCommand()
				{
					EntityId = _entityConfig.EntityId,
					Value = enabled,
				});
			};
		}
	}
}
