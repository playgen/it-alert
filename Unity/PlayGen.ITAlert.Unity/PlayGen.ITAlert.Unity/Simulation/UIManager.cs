using System;
using Engine.Lifecycle;
using Engine.Systems.Timing;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scoring.Team;
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
		private GameObject _endGameSuccessOverlay;

		[SerializeField]
		private GameObject _endGameNeutralOverlay;

		[SerializeField]
		private GameObject _endGameFailureOverlay;

		[SerializeField]
		private GameObject _endGameOverlay;

		[SerializeField]
		private Director _director;

		[SerializeField]
		private GameObject _teamSCoreOverlay;

		[SerializeField]
		private Text _teamScoreText;

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
					_endGameSuccessOverlay.SetActive(true);
					break;
				case EndGameState.Neutral:
					_endGameNeutralOverlay.SetActive(true);
					break;
				case EndGameState.Failure:
					_endGameFailureOverlay.SetActive(true);
					break;
				default:
					LogProxy.Error("Director ended game with unknown state");
					return;
			}
			_endGameOverlay.SetActive(true);

			if (_director.SimulationRoot.Scenario.Scoring != SimulationScenario.ScoringMode.None)
			{
				_teamSCoreOverlay.SetActive(true);
				if (_director.SimulationRoot.ECS.TryGetSystem<TeamScoringSystem>(out var teamScoringSystem))
				{
					_teamScoreText.text = teamScoringSystem.CumulativeScore.ToString("d5");
				}
			}
			_timerText.transform.parent.gameObject.SetActive(false);
			_itemPanel.SetActive(false);
		}

		private void FixedUpdate()
		{
			SetTimer();
		}

		#region read game params

		private void SetTimer()
		{
			_timerText.text = _director.SimulationRoot.ECS.TryGetSystem<TimerSystem>(out var timerSystem)
				&& timerSystem.Enabled
				? $"{timerSystem.Current.Minutes:00}:{timerSystem.Current.Seconds:00}"
				: _director.Tick.ToString("d5");
		}

		#endregion

		public void Reset()
		{
			_endGameSuccessOverlay.SetActive(false);
			_endGameNeutralOverlay.SetActive(false);
			_endGameFailureOverlay.SetActive(false);
			_endGameOverlay.SetActive(false);
			_timerText.transform.parent.gameObject.SetActive(true);
			_itemPanel.SetActive(true);
			_timerText.text = 0.ToString("d5");
			_teamSCoreOverlay.SetActive(false);
			_teamScoreText.text = 0.ToString("d5");
		}
	}
}