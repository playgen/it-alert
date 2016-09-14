using System;
using GameWork.Commands.Interfaces;
using PlayGen.ITAlert.Network;

public class JoinGameController : ICommandAction
{
	private readonly ITAlertClient _client;
	public event Action JoinGameSuccessEvent;
	public event Action<string> JoinGameFailedEvent;

	public JoinGameController(ITAlertClient client)
	{
		_client = client;
	}

	public void QuickMatch()
	{
		_client.JoinRandomRoom();
	}

	public void JoinGame(string name)
	{
		_client.JoinRoom(name);
	}
}
