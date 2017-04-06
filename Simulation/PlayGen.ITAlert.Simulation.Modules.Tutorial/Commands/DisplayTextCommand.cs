using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands
{
	public class DisplayTextCommand : ICommand
	{
		public string Text { get; set; }

		public bool Continue { get; set; }
	}

	public class DisplayTextCommandHandler : CommandHandler<DisplayTextCommand>
	{
		private readonly IEntityFactoryProvider _entityFactoryProvider;

		public DisplayTextCommandHandler(IEntityFactoryProvider entityFactoryProvider)
		{
			_entityFactoryProvider = entityFactoryProvider;
		}

		protected override bool TryProcessCommand(DisplayTextCommand command)
		{
			// TODO: possibly move this implementation into the tutorial system
			Entity textEntity;
			Text text;
			if (_entityFactoryProvider.TryCreateEntityFromArchetype(SimulationConstants.TutorialTextArchetype, out textEntity)
				&& textEntity.TryGetComponent(out text))
			{
				text.Value = command.Text;
				text.ShowContinue = command.Continue;
				return true;
			}
			textEntity?.Dispose();
			return false;
		}
	}
}
