using UnityEngine;
using System.Collections;
using GameWork.Core.Commands.Interfaces;

public class LeaveRoomCommand : ICommand<LobbyController>
{
    public void Execute(LobbyController parameter)
    {
        parameter.LeaveLobby();
    }
}
