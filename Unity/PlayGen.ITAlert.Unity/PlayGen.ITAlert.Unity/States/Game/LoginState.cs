using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.SUGAR.Unity;

namespace PlayGen.ITAlert.Unity.GameStates.Game
{
	public class LoginState : TickState, ICompletable
	{
		public const string StateName = nameof(LoginState);

		public override string Name => StateName;

		public bool IsComplete => SUGARManager.CurrentUser != null;

		protected override void OnEnter()
		{
			if (SUGARManager.CurrentUser == null)
			{
				SUGARManager.Account.DisplayPanel(success =>
				{
					if (success)
					{
						UnityEngine.Debug.Log("Sugar login successful.");
					}
					else
					{
						UnityEngine.Debug.LogError("Sugar login failed to login.");
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