using Engine.Commands;
using PlayGen.ITAlert.Simulation.Systems.Players;

namespace PlayGen.ITAlert.Simulation.Commands
{
	public class PlayerDisconnectedCommand : ICommand
	{
		public int PlayerExternalId { get; set; }
	}

	public class PlayerDisconnectedCommandHandler : CommandHandler<PlayerDisconnectedCommand>
	{
		private readonly PlayerSystem _playerSystem;

		public PlayerDisconnectedCommandHandler(PlayerSystem playerSystem)
		{
			_playerSystem = playerSystem;
		}


		protected override bool TryHandleCommand(PlayerDisconnectedCommand command, int currentTick, bool handlerEnabled)
		{
			_playerSystem.PlayerDisconnected(command.PlayerExternalId);
			return true;
		}
	}
}
