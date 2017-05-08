using System;
using GameWork.Core.States.Event;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.ITAlert.Unity.Photon;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.Transitions.GameExceptionChecked
{
	public class OnMessageTransition : EventStateTransition
	{
		private readonly ITAlertPhotonClient _photonClient;
		private readonly ITAlertChannel _channel;
		private readonly Type _requiredMessageType;
		private readonly string _toStateName;

		public OnMessageTransition(ITAlertPhotonClient photonClient, ITAlertChannel channel, Type messageType, string toStateName)
		{
			_photonClient = photonClient;
			_channel = channel;
			_requiredMessageType = messageType;
			_toStateName = toStateName;
		}

		protected override void OnEnter(string fromStateName)
		{
			_photonClient.CurrentRoom.Messenger.Subscribe((int)_channel, OnMessageRecieved);
		}

		protected override void OnExit(string toStateName)
		{
			_photonClient.CurrentRoom?.Messenger.Unsubscribe((int) _channel, OnMessageRecieved);
		}

		private void OnMessageRecieved(Message message)
		{
			if (!GameExceptionHandler.HasException && message.GetType() == _requiredMessageType)
			{
				ExitState(_toStateName);
				EnterState(_toStateName);
			}
		}
	}
}
