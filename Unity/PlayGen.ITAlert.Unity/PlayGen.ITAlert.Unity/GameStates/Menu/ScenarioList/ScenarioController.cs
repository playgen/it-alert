using System;
using GameWork.Core.Commands.Interfaces;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.GameStates.Menu.ScenarioList
{
	public class ScenarioController : ICommandAction
	{
		private readonly Client _photonClient;
		private ScenarioInfo _selected;
		public ScenarioInfo Selected => _selected;

		public event Action ScenarioSelectedSuccessEvent;
		public event Action<ScenarioInfo[]> ScenarioListSuccessEvent;

		public ScenarioController(Client photonClient)
		{
			_photonClient = photonClient;
		}

		//to-do: Get actual list from server
		public void GetScenarioList()
		{
			var scenarios = new ScenarioInfo[]
								{
									new ScenarioInfo("Large", 3, 6, "Game for many players"),
									new ScenarioInfo("Small", 1, 1, "Game for one"),
									new ScenarioInfo("Pair", 1, 2, "Game for two"),
								};
			ScenarioListSuccessEvent(scenarios);
		}

		public void SelectScenario(ScenarioInfo scenario)
		{
			_selected = scenario;
			ScenarioSelectedSuccessEvent();
		}

	}
}