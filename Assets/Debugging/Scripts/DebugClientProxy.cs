using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Simulation;

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
