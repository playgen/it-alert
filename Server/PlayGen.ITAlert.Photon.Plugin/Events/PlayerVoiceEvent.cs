using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using PlayGen.ITAlert.Simulation.Events;

namespace PlayGen.ITAlert.Photon.Plugin.Events
{
	public class PlayerVoiceEvent : PlayerEvent
	{
		public enum Signal
		{
			Error = 0,
			Activated,
			Deactivated,
		}

		public Signal Mode { get; set; }
	}
}
