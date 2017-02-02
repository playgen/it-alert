using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Tutorial;

namespace PlayGen.ITAlert.Simulation.Commands.Tutorial
{
	public class HideTextCommand : ICommand
	{
		public string Text { get; set; }

		public int? TextEntityId { get; set; }
	}

	public class HideTextCommandHandler : CommandHandler<HideTextCommand>
	{
		private ComponentMatcherGroup<Text> _textMatcher;

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
			}
			return false;
		}
	}
}
