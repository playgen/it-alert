
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;

namespace PlayGen.ITAlert.Simulation.Events
{
	public interface ISubsystemEvent : IEvent
	{
		int SubsystemEntityId { get; set; }
	}
}
