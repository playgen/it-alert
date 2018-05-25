using Engine.Lifecycle;
using Photon.Hive.Plugin;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.Photon.Analytics;
using PlayGen.Photon.Plugin;

namespace PlayGen.ITAlert.Photon.Plugin.RoomStates
{
	public class ExitState : ITAlertRoomState
	{
		public const string StateName = nameof(ExitState);

		public override string Name => StateName;

		private readonly SimulationLifecycleManager _simulationLifecycleManager;

		public ExitState(SimulationLifecycleManager simulationLifecycleManager,
			PluginBase photonPlugin,
			Messenger messenger,
			ITAlertPlayerManager playerManager,
			RoomSettings roomSettings,
			AnalyticsServiceManager analytics)
			: base(photonPlugin,
				  messenger,
				  playerManager,
				  roomSettings,
				  analytics)
		{
			_simulationLifecycleManager = simulationLifecycleManager;
		}

		protected override void OnEnter()
		{
			Shutdown(true);
		}

		protected override void OnExit()
		{
			
		}

		private void Shutdown(bool dispose)
		{
			_simulationLifecycleManager.TryStop();

			if (dispose)
			{
				_simulationLifecycleManager.Dispose();
			}
		}
	}
}
