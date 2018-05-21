using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Dev
{
    internal class Dev_SPL3_1sec : OverrideTimeLimit
    {
        public Dev_SPL3_1sec() : base(new SPL3(1))
        {
        }
    }
}
