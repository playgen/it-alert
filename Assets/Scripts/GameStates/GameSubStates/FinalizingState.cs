using GameWork.States;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class FinalizingState : TickableSequenceState
    {
        public const string StateName = "Finalizing";

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
        }

        public override void PreviousState()
        {
        }

        public override void Tick(float deltaTime)
        {
        }
    }
}