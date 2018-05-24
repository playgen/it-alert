using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlayGen.ITAlert.Unity.Simulation.Summary
{
    public static class SummaryMetricConfigs
    {
        public static SummaryMetricConfig Spoke = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_SPOKE",
            HighlightHighest = true,
            IconPath = "Sound_Icon"
        };

        public static SummaryMetricConfig Moved = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_MOVED",
            HighlightHighest = true,
            IconPath = "moved"
        };

        public static SummaryMetricConfig TransfersSent = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_TRANSFERS_SENT",
            HighlightHighest = true,
            IconPath = "transferitem_sent"
        };

        public static SummaryMetricConfig TransfersRecieved = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_TRANSFERS_RECEIVED",
            HighlightHighest = true,
            IconPath = "transferitem_received"
        };

        public static SummaryMetricConfig AntivirusesUsed = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_ANTIVURUSES_USED",
            HighlightHighest = true,
            IconPath = "antivirus"
        };

        public static SummaryMetricConfig ScannersUsed = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_SCANNERS_USED",
            HighlightHighest = true,
            IconPath = "scanner"
        };

        public static SummaryMetricConfig VirusesKilled = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_VIRUSES_KILLED",
            HighlightHighest = true,
            IconPath = "virus_killed"
        };

        public static SummaryMetricConfig AntivirusesWasted = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_ANTIVIRUSES_WASTED",
            HighlightHighest = true,
            IconPath = "antivirus_wasted"
        };

        public static SummaryMetricConfig VirusesFound = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_VIRUSES_FOUND",
            HighlightHighest = true,
            IconPath = "virus_found"
        };

        public static SummaryMetricConfig ScansWithNoVirusesFound = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_SCANS_WITH_NO_VIRUSES_FOUND",
            HighlightHighest = true,
            IconPath = "scan_no_virus_found"
        };

        public static SummaryMetricConfig CapturesUsed = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_CAPTURES_USED",
            HighlightHighest = true,
            IconPath = "capture"
        };

        public static SummaryMetricConfig VirusesCaptured = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_VIRUSES_CAPTURED",
            HighlightHighest = true,
            IconPath = "virus_captured"
        };

        public static SummaryMetricConfig CaptureWithNoVirusCaught = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_CAPTURE_WITH_NO_VIRUS_CAUGHT",
            HighlightHighest = true,
            IconPath = "capture_no_virus_caught"
        };

        public static SummaryMetricConfig AnalysersUsed = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_ANALYSERS_USED",
            HighlightHighest = true,
            IconPath = "analyser"
        };

        public static SummaryMetricConfig AntivirusesCreated = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_ANTIVIRUSES_CREATED",
            HighlightHighest = true,
            IconPath = "antivirus_created"
        };

        public static SummaryMetricConfig AntivirusCreationFails = new SummaryMetricConfig
        {
            Key = "SIMULATION_SUMMARY_METRIC_ANTIVIRUSES_CREATION_FAILS",
            HighlightHighest = true,
            IconPath = "antivirus_creation_failed"
        };

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