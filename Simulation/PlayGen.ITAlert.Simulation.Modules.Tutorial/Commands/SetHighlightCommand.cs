using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Commands;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands
{
	[Deduplicate(DeduplicationPolicy.Discard)]
	public class SetHighlightCommand : ICommand
	{
		public int EntityId { get; set; }

		public bool Value { get; set; }
	}

	public class SetHighlightCommandHandler : CommandHandler<SetHighlightCommand>
	{
		private readonly ComponentMatcherGroup<TutorialHighlight> _highlightMatcherGroup;

		public override IEqualityComparer<ICommand> Deduplicator => new DropItemCommandHandler.DropItemCommandEqualityComparer();

		public SetHighlightCommandHandler(IMatcherProvider matcherProvider)
		{
			_highlightMatcherGroup = matcherProvider.CreateMatcherGroup<TutorialHighlight>();
		}

		protected override bool TryProcessCommand(SetHighlightCommand command)
		{
			if (_highlightMatcherGroup.TryGetMatchingEntity(command.EntityId, out var highlightTuple))
			{
				highlightTuple.Component1.Enabled = command.Value;
				return true;
			}

			return false;
		}
	}
	public class DropItemCommandEqualityComparer : CommandEqualityComparer<SetHighlightCommand>
	{
		#region Overrides of CommandEqualityComparer<SetActorDestinationCommand>

		protected override bool IsDuplicate(SetHighlightCommand x, SetHighlightCommand other)
		{
			// player can only ever have one destination
			return x.EntityId == other.EntityId;
		}

		#endregion
	}
}
