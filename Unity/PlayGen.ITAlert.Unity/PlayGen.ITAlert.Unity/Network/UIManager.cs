using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace PlayGen.ITAlert.Unity.Network
{
	// ReSharper disable once CheckNamespace
	// ReSharper disable once InconsistentNaming
	public class UIManager : MonoBehaviour
	{
		#region editor properties

		/// <summary>
		/// timer text component
		/// </summary>
		[SerializeField] private Text _timerText;

		//[SerializeField]
		//private SVGImage _hintObjectIcon;

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

			SetScore();
			SetTimer();
		}

		#region read game params

		private void SetScore()
		{
			//_scoreText.text = Director.GetScore();
		}

		private void SetTimer()
		{
			_timerText.text = Director.GetTimer();
		}

		#endregion

	}
}