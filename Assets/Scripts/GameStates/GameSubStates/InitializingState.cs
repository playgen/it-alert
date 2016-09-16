using GameWork.States;
using PlayGen.ITAlert.Network;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class InitializingState : TickableSequenceState
    {
        public const string StateName = "Initializing";

        private readonly ITAlertClient _networkClient;

        public override string Name
        {
            get { return StateName; }
        }

        public InitializingState(ITAlertClient networkClient)
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
                Director.Tick();

                _networkClient.SetGameInitialized();
            }
        }
    }
}