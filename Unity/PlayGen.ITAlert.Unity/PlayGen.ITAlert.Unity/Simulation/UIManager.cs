using Engine.Lifecycle;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation
{
	// ReSharper disable once InconsistentNaming
	public class UIManager : MonoBehaviour
	{
		#region editor properties

		/// <summary>
		/// timer text component
		/// </summary>
		[SerializeField]
		private Text _timerText;

		[SerializeField]
		private GameObject _successOverlay;

		[SerializeField]
		private GameObject _failureOverlay;

		#endregion

		private void Awake()
		{
			Director.Reset += Reset;
			Director.GameEnded += Director_GameEnded;
		}

		private void Director_GameEnded(EndGameState endGameState)
		{
			switch (endGameState)
			{
				case EndGameState.Success:
					ShowSuccess();
					break;
				case EndGameState.Failure:
					ShowFailure();
					break;
				default:
					Debug.LogError("Director ended game with unknown state");
					break;
			}
		}

		private void FixedUpdate()
		{
			SetTimer();
		}

		#region read game params

		private void SetTimer()
		{
			_timerText.text = Director.Tick.ToString("d4");
		}

		#endregion

		public void ShowSuccess()
		{
			_successOverlay.SetActive(true);
		}

		public void ShowFailure()
		{
			_failureOverlay.SetActive(true);
		}

		public void Reset()
		{
			_successOverlay.SetActive(false);
			_failureOverlay.SetActive(false);
			_timerText.text = 0.ToString("d4");
		}
	}
}