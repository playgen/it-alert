using GameWork.States;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class PlayingState : TickableSequenceState
    {
        public const string StateName = "Playing";

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
            ChangeState(FinalizingState.StateName);
        }

        public override void PreviousState()
        {
        }

        public override void Tick(float deltaTime)
        {
        }
    }
}