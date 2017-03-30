using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Paused
{
	public class PausedState : InputTickState
	{
		public const string StateName = "Paused";
		public override string Name => StateName;
		
		private readonly Client _networkPhotonClient;

		public PausedState(TickStateInput input, Client networkPhotonClient) : base(input)
		{
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