using System;
using System.Collections.Generic;

namespace PlayGen.ITAlert.Photon.Messages.Simulation.States
{
	public class StopMessage : SimulationMessage
	{
	    public List<SimulationEvent> SimulationEvents;

        // Todo find a better place to put this - if we create a WebAPI for the logging DB this should be a DTO
	    public class SimulationEvent
	    {
	        public int? PlayerId { get; set; }

	        public string Data { get; set; }

	        public string EventCode { get; set; }

	        public int Tick { get; set; }
        }
	}
}
