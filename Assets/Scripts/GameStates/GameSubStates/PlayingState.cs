using GameWork.States;
using PlayGen.ITAlert.Network;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class PlayingState : TickableSequenceState
    {
        public const string StateName = "Playing";

        private readonly ITAlertClient _networkClient;

        public override string Name
        {
            get { return StateName; }
        }

        public PlayingState(ITAlertClient networkClient)
        {
            _networkClient = networkClient;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void NextState()
        {
            ChangeState(FinalizingState.StateName);
        }

        public override void PreviousState()
        {
        }

        public override void Tick(float deltaTime)
        {
            if (_networkClient.HasSimulationState)
            {
                Director.UpdateSimulation(_networkClient.TakeSimulationState());
                Director.Refresh();
            }

            // todo simulation UI tick 
        }
    }
}