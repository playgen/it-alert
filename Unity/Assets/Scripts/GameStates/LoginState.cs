
using GameWork.Core.States.Tick;
using PlayGen.SUGAR.Unity;

public class LoginState : TickState
{
	public const string StateName = "LoginState";

	protected override void OnEnter()
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

	protected override void OnExit()
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
