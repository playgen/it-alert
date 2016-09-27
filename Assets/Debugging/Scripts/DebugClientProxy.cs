using PlayGen.ITAlert.Network.Client;

namespace Assets.Debugging.Scripts
{
	public class DebugClientProxy : Client
	{
		public DebugClientProxy() 
			: base(new PhotonClient())
		{
		}

		public override void SendGameCommand(PlayGen.ITAlert.Simulation.Commands.Interfaces.ICommand command)
		{
			Director.LocaResolver.ProcessCommand(command);

		}
	}
}
