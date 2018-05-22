using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayGen.ITAlert.Unity.Simulation.Summary
{
    public class SummaryMetricConfig
    {
        public string Key { get; set; }

        public string IconPath { get; set; }

        // False = lowest is best, Null = no best.
        public bool? HighlightHighest { get; set; }
    }
}
