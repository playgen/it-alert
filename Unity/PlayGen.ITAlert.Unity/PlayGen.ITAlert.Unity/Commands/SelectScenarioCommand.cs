using Engine.Configuration;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scenario;
using PlayGen.ITAlert.Unity.Controllers;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class SelectScenarioCommand : ICommand<ScenarioController>
	{
		private readonly ScenarioInfo _scenarioInfo;

		public SelectScenarioCommand(ScenarioInfo scenarioInfo)
		{
			_scenarioInfo = scenarioInfo;
		}

		public void Execute(ScenarioController scenarioController)
		{
			scenarioController.SelectScenario(_scenarioInfo);
		}
	}
}
