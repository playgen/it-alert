using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Unity.Utilities;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game.Room.Initializing
{
	public class InitializingStateInput : TickStateInput
	{
		private GameObject _gameContainer;

		protected override void OnInitialize()
		{
			_gameContainer = GameObjectUtilities.FindGameObject("Game/Canvas/Graph");
		}


	}
}
