using GameWork.Core.States;

using PlayGen.SUGAR.Unity;

public class LoginState : TickState
{
	public const string StateName = "LoginState";

	public override void Enter()
	{
		SUGARManager.Account.DisplayPanel(success =>
		{
			if (success)
			{
				// todo refactor states
				//NextState();
			}
		});
	}

	public override void Exit()
	{
		SUGARManager.Account.HidePanel();
	}
	
	public override string Name
	{
		get { return StateName; }
	}

	//public override void NextState()
	//{
	// todo refactor states
	//	ChangeState(MenuState.StateName);
	//}
}
