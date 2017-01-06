﻿using Photon.Hive.Plugin;
using PlayGen.Photon.Players;
using PlayGen.Photon.SUGAR;

namespace PlayGen.Photon.Plugin.States
{
	public abstract class RoomState : GameWork.Core.States.State
	{
		protected readonly PluginBase PhotonPlugin;
		protected readonly Messenger Messenger;
		protected readonly PlayerManager PlayerManager;
		protected readonly Controller SugarController;

		protected RoomState(PluginBase photonPlugin, Messenger messenger, PlayerManager playerManager, Controller sugarController)
		{
			PhotonPlugin = photonPlugin;
			Messenger = messenger;
			PlayerManager = playerManager;
			SugarController = sugarController;
		}

		public virtual void OnRaiseEvent(IRaiseEventCallInfo info)
		{
		}

		public virtual void OnCreate(ICreateGameCallInfo info)
		{
		}

		public virtual void OnJoin(IJoinGameCallInfo info)
		{
		}

		public virtual void OnLeave(ILeaveGameCallInfo info)
		{
		}
	}
}