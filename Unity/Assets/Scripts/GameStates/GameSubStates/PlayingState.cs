using GameWork.Core.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class PlayingState : TickableSequenceState
    {
        public const string StateName = "Playing";

        private readonly Client _networkClient;

        public override string Name
        {
            get { return StateName; }
        }

        public PlayingState(Client networkClient)
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
            if (_networkClient.CurrentRoom.CurrentGame.HasSimulationState)
            {
                Director.UpdateSimulation(_networkClient.CurrentRoom.CurrentGame.TakeSimulationState());
                Director.Refresh();
            }
        }
    }
}