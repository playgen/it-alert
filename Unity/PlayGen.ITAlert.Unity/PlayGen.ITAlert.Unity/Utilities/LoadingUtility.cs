using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlayGen.ITAlert.Unity.Behaviours;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public static class LoadingUtility
	{
		public static LoadingSpinnerBehaviour LoadingSpinner;

		public static void SetSpinner(int speed, bool clockwise)
		{
			LoadingSpinner?.SetSpinner(clockwise, speed);
		}

		public static void ShowSpinner()
		{
			LoadingSpinner?.StartSpinner();
		}

		public static void HideSpinner()
		{
			LoadingSpinner?.StopSpinner();
		}
	}
}
