﻿using System;
using GameWork.Core.Commands.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.GamesList
{
	public class GamesListController : ICommandAction
	{
		private readonly Client _photonClient;
		public event Action<RoomInfo[]> GamesListSuccessEvent;

		public GamesListController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		public void GetGamesList()
		{
			var rooms = _photonClient.ListRooms(ListRoomsFilters.Open | ListRoomsFilters.Visible);
			GamesListSuccessEvent(rooms);
		}

	}
}