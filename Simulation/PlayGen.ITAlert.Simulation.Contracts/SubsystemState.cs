using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Contracts
{
	public class SubsystemState : NodeState
	{
        #region setup

        public int X { get; set; }
        public int Y { get; set; }

        public string Name { get; set; }

        #endregion

        #region UI

        public float Health { get; set; }

        public float Shield { get; set; }

        public bool Disabled { get; set; }

        public int? Infection { get; set; }

        public int?[] ItemPositions { get; set; }

		public int LogicalId { get; set; }

		public bool HasActiveItem { get; set; }

        #endregion

		#region Constructor

		public SubsystemState(int id) 
			: base(id, EntityType.Subsystem)
		{
		}

		#endregion
	}
}
