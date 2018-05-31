using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlayGen.ITAlert.Unity.Simulation.Summary
{
    public static class SummaryMetricConfigs
    {
        public static SummaryMetricConfig Spoke = new SummaryMetricConfig (
			"SPOKE", 
			"Sound_Icon", 
			true
		);

		public static SummaryMetricConfig Moved = new SummaryMetricConfig(
			"MOVED", 
			"moved", 
			true
		);

		public static SummaryMetricConfig TransfersSent = new SummaryMetricConfig(
			"TRANSFERS_SENT", 
			"transferitem_sent", 
			true
		);

		public static SummaryMetricConfig TransfersRecieved = new SummaryMetricConfig(
			"TRANSFERS_RECEIVED", 
			"transferitem_received", 
			true
		);

		public static SummaryMetricConfig AntivirusesUsed = new SummaryMetricConfig(
			"ANTIVIRUSES_USED", 
			"antivirus", 
			true
		);

		public static SummaryMetricConfig ScannersUsed = new SummaryMetricConfig(
			"SCANNERS_USED", 
			"scanner", 
			true
		);

		public static SummaryMetricConfig VirusesKilled = new SummaryMetricConfig(
			"VIRUSES_KILLED", 
			"virus_killed", 
			true
		);

		public static SummaryMetricConfig AntivirusesWasted = new SummaryMetricConfig(
			"ANTIVIRUSES_WASTED", 
			"antivirus_wasted", 
			false
		);

		public static SummaryMetricConfig VirusesFound = new SummaryMetricConfig(
			"VIRUSES_FOUND",
			"virus_found",
			true
		);

		public static SummaryMetricConfig ScansWithNoVirusesFound = new SummaryMetricConfig(
			"SCANS_WITH_NO_VIRUSES_FOUND", 
			"scan_no_virus_found", 
			false
		);

		public static SummaryMetricConfig CapturesUsed = new SummaryMetricConfig(
			"CAPTURES_USED", 
			"capture", 
			true
		);

		public static SummaryMetricConfig VirusesCaptured = new SummaryMetricConfig(
			"VIRUSES_CAPTURED", 
			"virus_captured", 
			true
		);

		public static SummaryMetricConfig CaptureWithNoVirusCaught = new SummaryMetricConfig(
			"CAPTURE_WITH_NO_VIRUS_CAUGHT", 
			"capture_no_virus_caught", 
			false
		);

		public static SummaryMetricConfig AnalysersUsed = new SummaryMetricConfig(
			"ANALYSERS_USED", 
			"analyser", 
			true
		);

		public static SummaryMetricConfig AntivirusesCreated = new SummaryMetricConfig(
			"ANTIVIRUSES_CREATED", 
			"antivirus_created", 
			true
		);

	    public static SummaryMetricConfig AntivirusCreationFails = new SummaryMetricConfig(
			"ANTIVIRUSES_CREATION_FAILS", 
			"antivirus_creation_failed", 
			false
		);
		
		private static IReadOnlyList<SummaryMetricConfig> _all;

        public static IReadOnlyList<SummaryMetricConfig> All => _all ?? (_all = GetAll());

        public static List<SummaryMetricConfig> GetAll()
        {
            var all = typeof(SummaryMetricConfigs)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType.IsAssignableFrom(typeof(SummaryMetricConfig)))
                .Select(f => (SummaryMetricConfig)f.GetValue(null))
                .ToList();

            return all;
        }
    }
}