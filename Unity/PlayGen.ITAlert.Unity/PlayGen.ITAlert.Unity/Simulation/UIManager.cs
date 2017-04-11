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
		private GameObject _itemPanel;

		[SerializeField]
		private GameObject _successOverlay;

		[SerializeField]
		private GameObject _failureOverlay;

		[SerializeField]
		private GameObject _endGameOverlay;

		[SerializeField]
		private Director _director;

		#endregion

		private void Start()
		{
			Canvas.ForceUpdateCanvases();
		}

		private void Awake()
		{
			_director.Reset += Reset;
			_director.GameEnded += Director_GameEnded;
		}

		private void Director_GameEnded(EndGameState endGameState)
		{
			switch (endGameState)
			{
				case EndGameState.Success:
					ShowEndScreen(true);
					break;
				case EndGameState.Failure:
					ShowEndScreen(false);
					break;
				default:
					LogProxy.Error("Director ended game with unknown state");
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
			_timerText.text = _director.Tick.ToString("d5");
		}

		#endregion

		public void ShowEndScreen(bool success)
		{
			if (success)
			{
				_successOverlay.SetActive(true);
			}
			else
			{
				_failureOverlay.SetActive(true);
			}
			_endGameOverlay.SetActive(true);
			_timerText.transform.parent.gameObject.SetActive(false);
			_itemPanel.SetActive(false);
			//_endGameOverlay.transform.Find("Team Score/Score").GetComponent<Text>().text = ;
		}

		public void Reset()
		{
			_successOverlay.SetActive(false);
			_failureOverlay.SetActive(false);
			_endGameOverlay.SetActive(false);
			_timerText.transform.parent.gameObject.SetActive(true);
			_itemPanel.SetActive(true);
			_timerText.text = 0.ToString("d5");
		}
	}
}