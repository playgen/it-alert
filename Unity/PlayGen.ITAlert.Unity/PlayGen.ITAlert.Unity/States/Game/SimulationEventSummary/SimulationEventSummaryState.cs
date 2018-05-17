using System.Collections.Generic;
using GameWork.Core.States.Event.Input;
using GameWork.Core.States.Input;
using JetBrains.Annotations;
using PlayGen.ITAlert.Photon.Messages.Simulation.States;
using PlayGen.Photon.Unity;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationEventSummary
{
    public class SimulationEventSummaryState : InputEventState
    {
        public const string StateName = nameof(SimulationEventSummaryState);

        private readonly SimulationSummary _simulationSummary;

        public override string Name => StateName;

        public SimulationEventSummaryState(StateInput stateInput, SimulationSummary simulationSummary) : base(stateInput)
        {
            _simulationSummary = simulationSummary;
        }

        protected override void OnEnter()
        {
            Logger.LogDebug("Entered " + StateName);
        }

        protected override void OnExit()
        {
            _simulationSummary.ClearData();
            Logger.LogDebug("Cleared Simulation Summary.");
        }
    }
}
