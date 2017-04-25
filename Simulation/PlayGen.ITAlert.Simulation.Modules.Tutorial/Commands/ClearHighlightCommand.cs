using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Components;
using PlayGen.ITAlert.Simulation.Modules.Tutorial.Components;

namespace PlayGen.ITAlert.Simulation.Modules.Tutorial.Commands
{
	[Deduplicate(DeduplicationPolicy.Discard)]
	public class ClearHighlightCommand : ICommand
	{
		public bool Value { get; set; }
	}

	public class ClearHighlightCommandHandler : CommandHandler<ClearHighlightCommand>
	{
		private readonly ComponentMatcherGroup<TutorialHighlight> _highlightMatcherGroup;

		public ClearHighlightCommandHandler(IMatcherProvider matcherProvider)
		{
			_highlightMatcherGroup = matcherProvider.CreateMatcherGroup<TutorialHighlight>();
		}

		protected override bool TryProcessCommand(ClearHighlightCommand command, int currentTick)
		{
			foreach(var highlight in _highlightMatcherGroup.MatchingEntities.Where(h => h.Component1.Enabled))
			{
				highlight.Component1.Enabled = false;
				return true;
			}
			return false;
		}
	}
}
