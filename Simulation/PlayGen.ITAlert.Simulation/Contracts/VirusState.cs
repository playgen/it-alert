using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class VirusState : ITAlertEntityState
	{
		//TODO: this needs a better name
		public bool Active { get; set; }

		public bool Visible { get; set; }

		public VirusState(int id) 
			: base(id, EntityType.Npc)
		{
			
		}
	}
}
