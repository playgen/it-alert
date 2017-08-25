using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Simulation;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Paused
{
	public class PausedState : InputTickState
	{
		public const string StateName = "Paused";
		public override string Name => StateName;
		
		private readonly ITAlertPhotonClient _networkPhotonClient;

		private readonly Director _director;

		public PausedState(Director director, TickStateInput input, ITAlertPhotonClient networkPhotonClient) : base(input)
		{
			_director = director;
			_networkPhotonClient = networkPhotonClient;
		}

		protected override void OnEnter()
		{
			Logger.LogDebug("Entered " + StateName);
		}

		protected override void OnExit()
		{
		}

	}
}