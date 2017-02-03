using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once CheckNamespace
	// ReSharper disable once InconsistentNaming
	public class UIManager : MonoBehaviour
	{
		#region editor properties

		/// <summary>
		/// timer text component
		/// </summary>
		[SerializeField]
		private Text _timerText;

		#endregion

		//TODO: why are these coroutines?
		private static Coroutine _warningFlash;
		//private static Coroutine _hintMove;
		private static int _flashCount;

		private void Awake()
		{

		}

		private void FixedUpdate()
		{
			//if (_upgradeGen != null)
			//{
			//	//_upgradeText.text = _upgradeGen.Amount.ToString();
			//}
			//if (_repairGen != null)
			//{
			//	//_repairText.text = _repairGen.Amount.ToString();
			//}

			SetTimer();
		}

		#region read game params

		private void SetTimer()
		{
			_timerText.text = Director.GetTick().ToString("d4");
		}

		#endregion

	}
}