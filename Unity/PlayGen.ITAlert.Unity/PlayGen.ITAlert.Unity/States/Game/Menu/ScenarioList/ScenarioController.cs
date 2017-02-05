using System;
using Engine.Configuration;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Startup;
using PlayGen.Photon.Unity.Client;

namespace PlayGen.ITAlert.Unity.States.Game.Menu.ScenarioList
{
	public class ScenarioController : ICommandAction
	{
		private readonly Client _photonClient;
		private ScenarioInfo _selected;
		public ScenarioInfo Selected => _selected;

		public event Action ScenarioSelectedSuccessEvent;
		public event Action<ScenarioInfo[]> ScenarioListSuccessEvent;

		private readonly ScenarioLoader _scenarioLoader;

		public ScenarioController(Client photonClient)
		{
			_photonClient = photonClient;
			_scenarioLoader = new ScenarioLoader();
		}

		//to-do: Get actual list from server
		public void GetScenarioList()
		{
			var scenarios = _scenarioLoader.GetScenarioInfo();

			ScenarioListSuccessEvent?.Invoke(scenarios);
		}

		public void SelectScenario(ScenarioInfo scenario)
		{
			_selected = scenario;
			ScenarioSelectedSuccessEvent?.Invoke();
		}

	}
}