using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Tutorial;

namespace PlayGen.ITAlert.Simulation.Commands.Tutorial
{
	public class DisplayTextCommand : ICommand
	{
		public string Text { get; set; }

		public bool Continue { get; set; }

		#region Equality members

		protected bool Equals(DisplayTextCommand other)
		{
			return string.Equals(Text, other.Text) && Continue == other.Continue;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((DisplayTextCommand) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Text?.GetHashCode() ?? 0) * 397) ^ Continue.GetHashCode();
			}
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals(other as DisplayTextCommand);
		}

		#endregion

		#endregion
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
