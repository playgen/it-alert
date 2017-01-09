using GameWork.Core.Commands.Interfaces;
using PlayGen.ITAlert.Network.Client;

public class JoinGameController : ICommandAction
{
	private readonly Client _client;

	public JoinGameController(Client client)
	{
		_client = client;
	}
    
	public void JoinGame(string name)
	{
		_client.JoinRoom(name);
	}
}
