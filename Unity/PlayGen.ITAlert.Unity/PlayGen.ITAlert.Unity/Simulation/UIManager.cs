using Engine.Lifecycle;
using Engine.Systems.Timing;
using PlayGen.ITAlert.Simulation.Configuration;
using PlayGen.ITAlert.Simulation.Scoring.Team;
using PlayGen.ITAlert.Unity.Behaviours;
using PlayGen.ITAlert.Unity.Exceptions;
using PlayGen.ITAlert.Unity.Utilities;

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

		private TimerSystem _timerSystem;

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

			var gameContainer = GameObjectUtilities.FindGameObject("Game/Canvas");

			var canvasGroup = gameContainer.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0.1f;
			canvasGroup.blocksRaycasts = false;
			foreach (var trail in gameContainer.GetComponentsInChildren<TrailRenderer>())
			{
				trail.startColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.25f);
				trail.endColor = new Color(trail.startColor.r, trail.startColor.g, trail.startColor.b, 0.125f);
			}
			foreach (var blink in gameContainer.GetComponentsInChildren<BlinkBehaviour>())
			{
				var image = blink.GetComponent<Image>();
				if (image != null)
				{
					blink.enabled = false;
					image.color = new Color(image.color.r, image.color.g, image.color.b, 0.625f);
				}
			}
		}

		private void FixedUpdate()
		{
			SetTimer();
		}

		#region read game params

		private void SetTimer()
		{
			if (_timerSystem == null)
			{
				if (_director.SimulationRoot.ECS.TryGetSystem(out _timerSystem) == false)
				{
					throw new SimulationIntegrationException("Could not locate timing system");
				}
			}
			_timerText.text = _timerSystem != null && _timerSystem.Enabled ? $"{_timerSystem.Current.Minutes:00}:{_timerSystem.Current.Seconds:00}" : _director.Tick.ToString("d5");
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
			_timerSystem = null;
		}
	}
}