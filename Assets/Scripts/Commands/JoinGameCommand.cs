using GameWork.Commands.Interfaces;

public class JoinGameCommand : ICommand
{
	private string _name;

	public JoinGameCommand(string name)
	{
		_name = name;
	}
	public void Execute(object parameter)
	{
		var controller = (JoinGameController)parameter;
		controller.JoinGame(_name);
	}
}
