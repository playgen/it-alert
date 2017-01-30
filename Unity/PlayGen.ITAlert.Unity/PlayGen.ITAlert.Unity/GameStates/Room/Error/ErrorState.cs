using System;
using GameWork.Core.States.Tick.Input;
using PlayGen.Photon.Unity;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Room.Error
{
	public class ErrorState : InputTickState
	{
		public const string StateName = "Error";
		public override string Name => StateName;

		private readonly Client _photonClient;

		public ErrorState(ErrorStateInput input, Client photonClient) 
			: base(input)
		{
			_photonClient = photonClient;
			input.BackClickedEvent += InputOnBackClickedEvent;

		}

		private void InputOnBackClickedEvent()
		{
			_photonClient.CurrentRoom.Leave();
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