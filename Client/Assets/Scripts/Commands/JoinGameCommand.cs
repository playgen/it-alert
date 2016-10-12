using GameWork.Core.Commands.Interfaces;

public class JoinGameCommand : ICommand<JoinGameController>
{
	private string _name;

	public JoinGameCommand(string name)
	{
		_name = name;
	}
	public void Execute(JoinGameController parameter)
	{
		parameter.JoinGame(_name);
	}
}
