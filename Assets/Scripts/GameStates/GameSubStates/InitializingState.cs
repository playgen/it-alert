using GameWork.States;
using PlayGen.ITAlert.Network;
using PlayGen.ITAlert.Network.Client;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class InitializingState : TickableSequenceState
    {
        public const string StateName = "Initializing";

        private readonly Client _networkClient;

        public override string Name
        {
            get { return StateName; }
        }

        public InitializingState(Client networkClient)
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
            ChangeState(PlayingState.StateName);
        }

        public override void PreviousState()
        {
        }

        public override void Tick(float deltaTime)
        {
            if (_networkClient.HasSimulationState)
            {
                Director.Initialize(_networkClient.TakeSimulationState(),
                    _networkClient.Player.ID);
                Director.Refresh();

                _networkClient.SetGameInitialized();
            }
        }
    }
}