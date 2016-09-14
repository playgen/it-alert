using GameWork.States;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class InitializingState : TickableSequenceState
    {
        public const string StateName = "Initializing";

        public override string Name
        {
            get { return StateName; }
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
            
        }
    }
}