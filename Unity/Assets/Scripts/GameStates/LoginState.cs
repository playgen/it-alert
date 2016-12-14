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
		SUGARManager.Account.TrySignIn(success =>
		{
			if (success)
			{
				NextState();
			}
		});
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
