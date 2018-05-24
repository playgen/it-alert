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

		public static SummaryMetricConfig AntivirusUsed = new SummaryMetricConfig(
			"ANTIVIRUS_USED", 
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

		public static SummaryMetricConfig AntivirusWasted = new SummaryMetricConfig(
			"ANTIVIRUS_WASTED", 
			"antivirus_wasted", 
			true
		);

		public static SummaryMetricConfig VirusesFound = new SummaryMetricConfig(
			"VIRUSES_FOUND",
			"virus_found",
			true
		);

		public static SummaryMetricConfig ScansWithNoVirusesFound = new SummaryMetricConfig(
			"SCANS_WITH_NO_VIRUSES_FOUND", 
			"scan_no_virus_found", 
			true
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
			true
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
			true
		);
    }
}