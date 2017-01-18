using System;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.Photon.SUGAR
{
	public class Controller : IDisposable
	{
		private readonly Configuration _sugarConfig;
		private readonly SUGARClient _sugarClient;
		private readonly AccountResponse _loggedInAccount;
		
		private bool _isDisposed;
		private MatchResponse _match;

		public Controller()
		{
			_sugarConfig = ConfigurationLoader.Load();
			_sugarClient = new SUGARClient(_sugarConfig.BaseAddress);
			_loggedInAccount = _sugarClient.Session.Login(_sugarConfig.GameId, new AccountRequest
			{
				Name = _sugarConfig.AccountName,
				Password = _sugarConfig.AccountPassword,
				SourceToken = _sugarConfig.AccountSource,
			});
		}

		~Controller()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_sugarClient?.Session.Logout();
		}

		public void StartMatch()
		{
			_match = _sugarClient.Match.CreateAndStart();
		}

		public void EndMatch()
		{
			_match = _sugarClient.Match.End(_match.Id);
		}

		public void AddMatchData(string key, long value, int? playerSugarId = null)
		{
			AddMatchData(key, value.ToString(), EvaluationDataType.Long, playerSugarId);
		}

		public void AddMatchData(string key, string value, int? playerSugarId = null)
		{
			AddMatchData(key, value, EvaluationDataType.String, playerSugarId);
		}

		public void AddMatchData(string key, bool value, int? playerSugarId = null)
		{
			AddMatchData(key, value.ToString(), EvaluationDataType.Boolean, playerSugarId);
		}

		public void AddMatchData(string key, float value, int? playerSugarId = null)
		{
			AddMatchData(key, value.ToString(), EvaluationDataType.Float, playerSugarId);
		}

		public void AddMatchRankingData(string category, int rank, int playerSugarId)
		{
			AddMatchData($"RANKING_{category}", rank, playerSugarId);
		}

		private void AddMatchData(string key, string value, EvaluationDataType dataType, int? playerSugarId = null)
		{
			if (playerSugarId == null)
			{
				playerSugarId = _loggedInAccount.User.Id;
			}

			_sugarClient.Match.AddData(new EvaluationDataRequest
			{
				CreatingActorId = playerSugarId,
				EvaluationDataType = dataType,
				GameId = _sugarConfig.GameId,
				Key = key,
				MatchId = _match.Id,
				Value = value
			});
		}
	}
}
