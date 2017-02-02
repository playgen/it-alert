using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.States.Game
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
					var autoLogIn = !SUGARManager.Account.IsActive;
					if (success)
					{
						IsComplete = true;
					}
					else if (autoLogIn)
					{
						_repeatBlock = false;
						OnEnter();
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