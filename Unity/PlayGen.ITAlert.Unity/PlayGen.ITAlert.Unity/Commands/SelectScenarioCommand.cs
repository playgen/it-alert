using System;
using Engine.Configuration;
using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Unity.Controllers;
using PlayGen.ITAlert.Unity.States.Game.Menu.ScenarioList;

namespace PlayGen.ITAlert.Unity.Commands
{
	public class SelectScenarioCommand : ICommand<ScenarioController>
	{
		private readonly ScenarioInfo _scenario;

		public SelectScenarioCommand(ScenarioInfo scen)
		{
			_scenario = scen;
		}

		public void Execute(ScenarioController parameter)
		{
			parameter.SelectScenario(_scenario);
		}
	}
}
