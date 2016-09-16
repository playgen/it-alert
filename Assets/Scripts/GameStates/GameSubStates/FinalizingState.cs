﻿using GameWork.States;
using PlayGen.ITAlert.Network;

namespace PlayGen.ITAlert.GameStates.GameSubStates
{
    public class FinalizingState : TickableSequenceState
    {
        public const string StateName = "Finalizing";

        private readonly ITAlertClient _networkClient;

        public override string Name
        {
            get { return StateName; }
        }

        public FinalizingState(ITAlertClient networkClient)
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
        }

        public override void PreviousState()
        {
        }

        public override void Tick(float deltaTime)
        {
            if (_networkClient.HasSimulationState)
            {
                Director.Finalise(_networkClient.TakeSimulationState());
                Director.Tick();
                _networkClient.SetGameFinalized();
            }

            // todo simulation UI tick Director.Tick();
        }
    }
}