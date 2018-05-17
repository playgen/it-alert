using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayGen.ITAlert.Simulation.Configuration.Scenarios.SPL;
using PlayGen.ITAlert.Simulation.Scenario;

namespace PlayGen.ITAlert.Simulation.Configuration.Scenarios.Dev
{
    internal abstract class OverrideTimeLimit : ScenarioFactory
    {
        private readonly ScenarioFactory _overrideScenarioFactory;

        protected OverrideTimeLimit(ScenarioFactory overrideScenarioFactory)
            : base(
                $"Dev {overrideScenarioFactory.ScenarioInfo.Key} {overrideScenarioFactory.ScenarioInfo.TimeLimitSeconds} seconds",
                $"Dev {overrideScenarioFactory.ScenarioInfo.Name} {overrideScenarioFactory.ScenarioInfo.TimeLimitSeconds} seconds",
                $"Dev {overrideScenarioFactory.ScenarioInfo.Description} {overrideScenarioFactory.ScenarioInfo.TimeLimitSeconds} seconds",
                overrideScenarioFactory.ScenarioInfo.MinPlayerCount,
                overrideScenarioFactory.ScenarioInfo.MaxPlayerCount,
                overrideScenarioFactory.ScenarioInfo.TimeLimitSeconds)
        {
            _overrideScenarioFactory = overrideScenarioFactory;
        }
        
        public override SimulationScenario GenerateScenario()
        {
            var scenario = _overrideScenarioFactory.GenerateScenario();
            scenario.Key = ScenarioInfo.Key;
            scenario.Name = ScenarioInfo.Name;
            scenario.Description = ScenarioInfo.Description;
            return scenario;
        }
    }
}