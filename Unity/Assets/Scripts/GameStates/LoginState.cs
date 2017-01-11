using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Interfaces;
using PlayGen.SUGAR.Unity;

public class LoginState : TickState, ICompletable
{
	public const string StateName = "LoginState";

	public bool IsComplete { get; private set; }

	protected override void OnEnter()
	{
		IsComplete = false;

		SUGARManager.Account.DisplayPanel(success =>
		{
			if (success)
			{
				IsComplete = true;
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
}
