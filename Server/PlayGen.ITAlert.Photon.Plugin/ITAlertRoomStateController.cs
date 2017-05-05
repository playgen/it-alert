using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Photon.Players;
using PlayGen.ITAlert.Photon.Plugin.RoomStates;
using PlayGen.Photon.Plugin;

namespace PlayGen.ITAlert.Photon.Plugin
{
	// ReSharper disable once InconsistentNaming
	public class ITAlertRoomStateController : RoomStateController<ITAlertRoomState, ITAlertPlayerManager, ITAlertPlayer>
	{
		public ITAlertRoomStateController(params ITAlertRoomState[] states) : base(states)
		{
		}
	}
}
