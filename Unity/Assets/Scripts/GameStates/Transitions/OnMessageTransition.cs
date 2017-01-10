using System;
using GameWork.Core.States.Event;
using PlayGen.ITAlert.Network.Client;
using PlayGen.ITAlert.Photon.Messages;
using PlayGen.Photon.Messaging;

namespace PlayGen.ITAlert.GameStates.Transitions
{
	public class OnMessageTransition : EventStateTransition
	{
		private readonly Client _client;
		private readonly Channels _channel;
		private readonly Type _requiredMessageType;
		private readonly string _toStateName;

		public OnMessageTransition(Client client, Channels channel, Type messageType, string toStateName)
		{
			_client = client;
			_channel = channel;
			_requiredMessageType = messageType;
			_toStateName = toStateName;
		}

		protected override void OnEnter()
		{
			_client.CurrentRoom.Messenger.Subscribe((int)_channel, OnMessageRecieved);
		}

		protected override void OnExit()
		{
			_client.CurrentRoom.Messenger.Unsubscribe((int)_channel, OnMessageRecieved);
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
