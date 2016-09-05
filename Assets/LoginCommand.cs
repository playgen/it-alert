using UnityEngine;
using System.Collections;
using GameWork.Commands.Interfaces;
using GameWork.States.Interfaces;

public struct LoginCommand : ICommand
{
    public void Execute(object parameter)
    {
        var state = (IState)parameter;
        //state.ChangeState(LoadingState.StateName);
    }
}

