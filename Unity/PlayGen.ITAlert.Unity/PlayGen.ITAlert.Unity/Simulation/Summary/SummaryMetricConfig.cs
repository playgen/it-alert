namespace PlayGen.ITAlert.Unity.Simulation.Summary
{
    public class SummaryMetricConfig
    {
		public string KeyMetric { get; set; }
		/// <summary>
		/// The title for having the best number of this metric
		/// </summary>
		public string KeyTitle { get; set; }

		/// <summary>
		/// The description players will see
		/// </summary>
	    public string KeyDescription { get; set; }

		/// <summary>
		/// The descriptive metric text, eg "{0} Virus Killed"
		/// </summary>
		public string KeyFormattedMetric { get; set; }

		public string IconPath { get; set; }

        // False = lowest is best, Null = no best.
        public bool? HighlightHighest { get; set; }

	    private const string _keyPrefix = "SIMULATION_SUMMARY_METRIC_";


		public SummaryMetricConfig(string keyExtension, string iconPath, bool? highlighHighest)
		{
			KeyMetric = _keyPrefix + keyExtension;
			KeyTitle = _keyPrefix + keyExtension + "_TITLE";
			KeyDescription = _keyPrefix + keyExtension + "_DESCRIPTION";
			KeyFormattedMetric = _keyPrefix + keyExtension + "_FORMATTED";

			IconPath = iconPath;
			HighlightHighest = highlighHighest;
		}
    }
}
