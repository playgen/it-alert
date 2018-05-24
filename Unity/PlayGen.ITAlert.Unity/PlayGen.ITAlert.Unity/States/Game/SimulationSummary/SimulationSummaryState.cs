using GameWork.Core.States.Tick.Input;

namespace PlayGen.ITAlert.Unity.States.Game.SimulationSummary
{
    public class SimulationSummaryState : InputTickState
    {
        public const string StateName = nameof(SimulationSummaryState);

        private readonly SimulationSummary _simulationSummary;

        public override string Name => StateName;

        public SimulationSummaryState(TickStateInput stateInput, SimulationSummary simulationSummary) : base(stateInput)
        {
            _simulationSummary = simulationSummary;
        }

        protected override void OnEnter()
        {
            UnityEngine.Debug.Log("Entered " + StateName);
        }

        protected override void OnExit()
        {
            _simulationSummary.ClearData();
            UnityEngine.Debug.Log("Cleared Simulation Summary.");
        }
    }
}
