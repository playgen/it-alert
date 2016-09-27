using PlayGen.ITAlert.Network.Client;

namespace Assets.Debugging.Scripts
{
	public class DebugClientProxy : ITAlertClient
	{
		public DebugClientProxy() 
			: base(new Client())
		{
		}

		public override void SendGameCommand(PlayGen.ITAlert.Simulation.Commands.Interfaces.ICommand command)
		{
			Director.LocaResolver.ProcessCommand(command);

		}
	}
}
