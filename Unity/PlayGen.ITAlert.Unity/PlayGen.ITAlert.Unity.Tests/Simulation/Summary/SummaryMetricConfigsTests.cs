using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PlayGen.ITAlert.Unity.Simulation.Summary;

namespace PlayGen.ITAlert.Unity.Tests.Simulation.Summary
{
    public class SummaryMetricConfigsTests
    {
        private static readonly IReadOnlyList<SummaryMetricConfig> ExpectedAll = new List<SummaryMetricConfig>
        {
            SummaryMetricConfigs.Spoke,
            SummaryMetricConfigs.Moved,
            SummaryMetricConfigs.TransfersSent,
            SummaryMetricConfigs.TransfersRecieved,
            SummaryMetricConfigs.AntivirusesUsed,
            SummaryMetricConfigs.ScannersUsed,
            SummaryMetricConfigs.VirusesKilled,
            SummaryMetricConfigs.AntivirusesWasted,
            SummaryMetricConfigs.VirusesFound,
            SummaryMetricConfigs.ScansWithNoVirusesFound,
            SummaryMetricConfigs.CapturesUsed,
            SummaryMetricConfigs.VirusesCaptured,
            SummaryMetricConfigs.CaptureWithNoVirusCaught,
            SummaryMetricConfigs.AnalysersUsed,
            SummaryMetricConfigs.AntivirusesCreated,
            SummaryMetricConfigs.AntivirusCreationFails
        };

        [Test]
        public void CanListAll()
        {
            Assert.AreEqual(ExpectedAll.Count, SummaryMetricConfigs.All.Count);
            Assert.IsTrue(ExpectedAll.All(ea => SummaryMetricConfigs.All.Any(a => ea.Key == a.Key)));
        }
    }
}
