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

		#region Equality members

		protected bool Equals(HideTextCommand other)
		{
			return string.Equals(Text, other.Text) && TextEntityId == other.TextEntityId;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((HideTextCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Text?.GetHashCode() ?? 0) * 397) ^ TextEntityId.GetHashCode();
			}
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals(other as HideTextCommand);
		}

		#endregion

		#endregion
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
