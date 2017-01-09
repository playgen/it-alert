using GameWork.Core.Commands.Interfaces;
using GameWork.Core.States.Tick.Input;
using PlayGen.ITAlert.Network.Client;

public class CreateGameState : InputTickState
{
    private readonly CreateGameController _controller;
    private readonly Client _client;
    public const string StateName = "CreateGameState";


    public CreateGameState(CreateGameStateInput input, CreateGameController controller) : base(input)
    {
        _controller = controller;
    }
 
    protected override void OnTick(float deltaTime)
    {
	    ICommand command;
        if (CommandQueue.TryTakeFirstCommand(out command))
        {
            var createGameCommand = command as CreateGameCommand;
            if (createGameCommand != null)
            {
                createGameCommand.Execute(_controller);
                return;
            }
            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }

    public override string Name
    {
        get { return StateName; }
    }

	// todo refactor states
    //public override void NextState()
    //{
    //    ChangeState(LobbyState.StateName);
    //}

    //public override void PreviousState()
    //{
    //    ChangeState(MainMenuState.StateName);
    //}
}