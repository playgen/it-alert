﻿using System;
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
			Entity textEntity;
			Text text;
			if (_entityFactoryProvider.TryCreateEntityFromArchetype(SimulationConstants.TutorialTextArchetype, out textEntity)
				&& textEntity.TryGetComponent(out text))
			{
				text.Value = command.Text;
				return true;
			}
			textEntity?.Dispose();
			return false;
		}
	}
}