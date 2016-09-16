using System;
using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.Network;

public class JoinGameController : ICommandAction
{
	private readonly ITAlertClient _client;

	public JoinGameController(ITAlertClient client)
	{
		_client = client;
	}
    
	public void JoinGame(string name)
	{
		_client.JoinRoom(name);
	}
}
