using GameWork.Core.States;

using PlayGen.SUGAR.Unity;

public class LoginState : TickableSequenceState
{
	public const string StateName = "LoginState";

	public override void PreviousState()
	{
		throw new System.NotImplementedException();
	}

	public override void Enter()
	{
		SUGARManager.Account.DisplayPanel(success =>
		{
			if (success)
			{
				NextState();
			}
		});
	}

	public override void Exit()
	{
		SUGARManager.Account.HidePanel();
	}

	public override void Tick(float deltaTime)
	{
		
	}

	public override string Name
	{
		get { return StateName; }
	}

	public override void NextState()
	{
		ChangeState(MenuState.StateName);
	}
}
