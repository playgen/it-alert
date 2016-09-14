using UnityEngine;
using System.Collections;
using GameWork.Commands.Interfaces;

public class LeaveRoomCommand : ICommand<LobbyController>
{
    public void Execute(LobbyController parameter)
    {
        parameter.LeaveLobby();
    }
}
