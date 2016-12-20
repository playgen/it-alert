using System;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.ITAlert.Photon.Plugins.SUGAR
{
    public class SUGARController : IDisposable
    {
        private readonly SUGARConfigurationSection _sugarConfig;
        private readonly SUGARClient _sugarClient;
        private readonly AccountResponse _loggedInAccount;
        
        private bool _isDisposed;
        private MatchResponse _match;

        public SUGARController()
        {
            _sugarConfig = SUGARConfigurationSection.GetSection();
            _sugarClient = new SUGARClient(_sugarConfig.BaseAddress);
            _loggedInAccount = _sugarClient.Session.Login(_sugarConfig.GameId, new AccountRequest
            {
                Name = _sugarConfig.AccountName,
                Password = _sugarConfig.AccountPassword,
                SourceToken = _sugarConfig.AccountSource,
            });
        }

        ~SUGARController()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _sugarClient.Session.Logout();
        }

        public void StartMatch()
        {
            _match = _sugarClient.Match.CreateAndStart();
        }

        public void EndMatch()
        {
            _match = _sugarClient.Match.End(_match.Id);
        }

        public void AddMatchData(string key, long value, int? playerExternalId = null)
        {
            AddMatchData(key, value.ToString(), EvaluationDataType.Long, playerExternalId);
        }

        public void AddMatchData(string key, string value, int? playerExternalId = null)
        {
            AddMatchData(key, value, EvaluationDataType.String, playerExternalId);
        }

        public void AddMatchData(string key, bool value, int? playerExternalId = null)
        {
            AddMatchData(key, value.ToString(), EvaluationDataType.Boolean, playerExternalId);
        }

        public void AddMatchData(string key, float value, int? playerExternalId = null)
        {
            AddMatchData(key, value.ToString(), EvaluationDataType.Float, playerExternalId);
        }

        private void AddMatchData(string key, string value, EvaluationDataType dataType, int? playerExternalId = null)
        {
            if (playerExternalId == null)
            {
                playerExternalId = _loggedInAccount.User.Id;
            }

            _sugarClient.Match.AddData(new EvaluationDataRequest
            {
                CreatingActorId = playerExternalId,
                EvaluationDataType = dataType,
                GameId = _sugarConfig.GameId,
                Key = key,
                MatchId = _match.Id,
                Value = value
            });
        }
    }
}
