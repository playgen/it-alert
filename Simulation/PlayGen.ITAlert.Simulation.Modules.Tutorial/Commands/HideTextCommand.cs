using System.Linq;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands
{
	public class HideTextCommand : ICommand
	{
		public string Text { get; set; }

		public int? TextEntityId { get; set; }
	}

	public class HideTextCommandHandler : CommandHandler<HideTextCommand>
	{
		private readonly ComponentMatcherGroup<Text> _textMatcher;

		public HideTextCommandHandler(IMatcherProvider matcherProvider)
		{
			_textMatcher = matcherProvider.CreateMatcherGroup<Text>();
		}

		protected override bool TryProcessCommand(HideTextCommand command)
		{
			ComponentEntityTuple<Text> tuple;
			if (command.TextEntityId.HasValue
				&& _textMatcher.TryGetMatchingEntity(command.TextEntityId.Value, out tuple))
			{
				tuple.Entity.Dispose();
				return true;
			}
			else
			{
				foreach (var textTuple in _textMatcher.MatchingEntities.ToArray())
				{
					textTuple.Entity.Dispose();
				}
				return true;
			}
			return false;
		}
	}
}
