using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Archetypes;
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

		protected override bool TryHandleCommand(DisplayTextCommand command, int currentTick, bool handlerEnabled)
		{
			if (handlerEnabled)
			{
				if (_entityFactoryProvider.TryCreateEntityFromArchetype(TutorialText.Archetype, out var textEntity)
					&& textEntity.TryGetComponent<Text>(out var text))
				{
					text.Value = command.Text;
					text.ShowContinue = command.Continue;
					return true;
				}
				textEntity?.Dispose();
			}
			return false;
		}
	}
}
