using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;

public class MainMenuState : InputTickState
{
	private readonly QuickGameController _controller;
	private readonly Client _client;
	public const string StateName = "MainMenuState";

	public MainMenuState(MenuStateInput input, QuickGameController controller, Client client) : base(input)
	{
		_controller = controller;
		_client = client;
	}

	public override string Name
	{
		get { return StateName; }
	}

	protected override void OnTick(float deltaTime)
	{
		ICommand command;
		if (CommandQueue.TryTakeFirstCommand(out command))
		{
			var quickGameCommand = command as QuickGameCommand;
			if (quickGameCommand != null)
			{
				quickGameCommand.Execute(_controller);
			}

			var commandResolver = new StateCommandResolver();
			commandResolver.HandleSequenceStates(command, this);
		}
	}
}
