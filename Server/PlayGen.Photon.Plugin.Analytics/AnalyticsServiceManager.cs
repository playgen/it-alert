using System;
using System.Collections.Generic;
using NLog;
using PlayGen.Photon.Plugin.Analytics.Interfaces;

namespace PlayGen.Photon.Plugin.Analytics
{
	public class AnalyticsServiceManager : IAnalyticsService
	{
		private readonly List<IAnalyticsService> _services = new List<IAnalyticsService>();
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public AnalyticsServiceManager(params IAnalyticsService[] services)
		{
			_services.AddRange(services);
		}

		public void StartMatch()
		{
			_logger.Debug("StartMatch()");
			_services.ForEach(s => TryExecute(s.StartMatch));
		}

		public void AddMatchData(string key, int value)
		{
			_logger.Debug($"AddMatchData(key: {key}, value: {value})");
			_services.ForEach(s => TryExecute(() => s.AddMatchData(key, value)));
		}

		public void EndMatch()
		{
			_logger.Debug("EndMatch()");
			_services.ForEach(s => TryExecute(s.EndMatch));
		}

		public void AddMatchRankingData(string category, int rank, int playerExternalId)
		{
			_logger.Debug($"AddMatchRankingData(category: {category}, rank: {rank}, playerExternalId: {playerExternalId})");
			_services.ForEach(s => TryExecute(() => s.AddMatchRankingData(category, rank, playerExternalId)));
		}

		private void TryExecute(Action action)
		{
			try
			{
				action();
			}
			catch (Exception exception)
			{
				_logger.Error(exception);
			}
		}
	}
}
