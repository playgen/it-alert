using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;
using Engine.Entities;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class CreatePlayerCommand : ICommand
	{
		public PlayerConfig PlayerConfig { get; set; }
	}

	public class CreatePlayerCommandHandler : CommandHandler<CreatePlayerCommand>
	{
		private readonly PlayerSystem _playerSystem;
		
		public CreatePlayerCommandHandler(PlayerSystem playerSystem)
		{
			_playerSystem = playerSystem;
		}

		protected override bool TryProcessCommand(CreatePlayerCommand command)
		{
			if (command.PlayerConfig == null) return false;

			var player = _playerSystem.CreatePlayer(command.PlayerConfig);
			return player != null;
		}
	}
}
