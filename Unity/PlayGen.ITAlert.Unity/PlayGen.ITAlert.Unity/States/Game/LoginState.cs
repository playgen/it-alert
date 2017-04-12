using GameWork.Core.States.Tick;
using PlayGen.ITAlert.Unity.Interfaces;
using PlayGen.SUGAR.Unity;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.States.Game
{
	public class LoginState : TickState, ICompletable
	{
		public const string StateName = nameof(LoginState);

		public override string Name => StateName;

		public bool IsComplete => SUGARManager.CurrentUser != null;

		protected override void OnEnter()
		{
			SugarLogin();
		}

		private void SugarLogin()
		{
			if (!IsComplete)
			{
				SUGARManager.Account.DisplayPanel(success =>
				{
					if (!success)
					{
						if (!SUGARManager.Account.IsActive)
						{
							SugarLogin();
						}
						else
						{
							LogProxy.Info("Sugar Login Failed");
						}
					}
				});
			}
		}

		protected override void OnTick(float deltaTime)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
		}

		protected override void OnExit()
		{
			SUGARManager.Account.Hide();
		}
	}
}