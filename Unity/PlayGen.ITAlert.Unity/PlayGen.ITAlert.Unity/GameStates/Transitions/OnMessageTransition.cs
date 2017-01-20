using System;
using GameWork.Core.States.Event;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.Photon.Messaging;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Transitions
{
	public class OnMessageTransition : EventStateTransition
	{
		private readonly Client _photonClient;
		private readonly Channel _channel;
		private readonly Type _requiredMessageType;
		private readonly string _toStateName;

		public OnMessageTransition(Client photonClient, Channel channel, Type messageType, string toStateName)
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
			if (_photonClient.CurrentRoom != null)
			{
				_photonClient.CurrentRoom.Messenger.Unsubscribe((int) _channel, OnMessageRecieved);
			}
		}

		private void OnMessageRecieved(Message message)
		{
			if (message.GetType() == _requiredMessageType)
			{
				ChangeState(_toStateName);
			}
		}
	}
}
