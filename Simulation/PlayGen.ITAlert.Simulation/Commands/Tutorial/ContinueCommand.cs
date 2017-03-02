using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Tutorial;
using PlayGen.ITAlert.Simulation.Systems.Tutorial;

namespace PlayGen.ITAlert.Simulation.Commands.Tutorial
{
	public class ContinueCommand : ICommand
	{
		public string Text { get; set; }

		#region Equality members

		protected bool Equals(ContinueCommand other)
		{
			return string.Equals(Text, other.Text);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ContinueCommand) obj);
		}

		public override int GetHashCode()
		{
			return Text?.GetHashCode() ?? 0;
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals(other as ContinueCommand);
		}

		#endregion

		#endregion
	}

	public class ContinueCommandCommandHandler : CommandHandler<ContinueCommand>
	{
		private readonly ITutorialSystem _tutorialSystem;

		public ContinueCommandCommandHandler(ITutorialSystem tutorialSystem)
		{
			_tutorialSystem = tutorialSystem;
		}

		protected override bool TryProcessCommand(ContinueCommand command)
		{
			_tutorialSystem.SetContinue();
			return true;
		}
	}
}
