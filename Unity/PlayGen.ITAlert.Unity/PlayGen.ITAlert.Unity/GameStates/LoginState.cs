using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.ITAlert.Unity.Utilities;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.GameStates
{
	public class LoginState : TickState, ICompletable
	{
		public const string StateName = "LoginState";

		public bool IsComplete { get; private set; }

	    private bool _repeatBlock;

		protected override void OnEnter()
		{
		    if (!_repeatBlock)
		    {
		        _repeatBlock = true;
		        IsComplete = false;

		        SUGARManager.Account.DisplayPanel(success =>
		        {
					var autoLogIn = SUGARManager.Account.IsActive;

					if (success)
		            {
		                IsComplete = true;
		            }
					else if (autoLogIn)
		            {
						//PopupUtility.LogError("Failed to log into the game. ");
		            }
		        });
		    }
		}

		protected override void OnExit()
		{
			SUGARManager.Account.Hide();
		}

		public override string Name
		{
			get { return StateName; }
		}
	}
}