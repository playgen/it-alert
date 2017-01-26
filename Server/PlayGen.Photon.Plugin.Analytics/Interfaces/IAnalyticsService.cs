namespace PlayGen.Photon.Plugin.Analytics.Interfaces
{
	public interface IAnalyticsService
	{
		void StartMatch();

		void AddMatchData(string key, int value);

		void EndMatch();

		void AddMatchRankingData(string category, int rank, int playerExternalId);
	}
}
