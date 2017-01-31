using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.GameStates.Game
{
	public class LoginState : TickState, ICompletable
	{
		public const string StateName = nameof(LoginState);

		public override string Name => StateName;

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
		            if (success)
		            {
		                IsComplete = true;
		            }
		        });
		    }
		}

		protected override void OnExit()
		{
			SUGARManager.Account.Hide();
		}
	}
}