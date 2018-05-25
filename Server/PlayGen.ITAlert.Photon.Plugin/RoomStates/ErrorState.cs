using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.Photon.Plugin;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Messages.Error;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class ErrorState : ITAlertRoomState
	{
		public const string StateName = nameof(ErrorState);

		private readonly ExceptionHandler _exceptionHandler;

		public override string Name => StateName;

		public ErrorState(PluginBase photonPlugin, 
			Messenger messenger, 
			ITAlertPlayerManager playerManager, 
			RoomSettings roomSettings,
			AnalyticsServiceManager analytics,
			ExceptionHandler exceptionHandler)
			: base(photonPlugin, messenger, playerManager, roomSettings, analytics)
		{
			_exceptionHandler = exceptionHandler;
		}

		protected override void OnEnter()
		{
			var message = "Server Error";

			if (_exceptionHandler.Exception != null)
			{
				message += $"\n {_exceptionHandler.Exception}";
			}

			Messenger.SendAllMessage(new ErrorMessage
			{
				Message = message
			});
		}
	}
}
