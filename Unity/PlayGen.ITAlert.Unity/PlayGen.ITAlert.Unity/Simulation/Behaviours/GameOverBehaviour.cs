using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class GameOverBehaviour : MonoBehaviour
	{
		public enum GameOverCondition
		{
			Success,
			Failure
		}
		[SerializeField]
		private GameObject _bigVirus;

		[SerializeField]
		private GameOverCondition _condition;
		private static readonly Vector3 InitialOffset = new Vector3(0, 1000);

		private float _animationProgress;

		// Use this for initialization
		private void Start()
		{
		}

		private void Awake()
		{

		}

		// Update is called once per frame
		private void Update () {
			if (_animationProgress < 1)
			{
				_animationProgress += Time.deltaTime;
				//LogProxy.Info(_animationProgress);
				_bigVirus.transform.localPosition = new Vector3(InitialOffset.x, Mathf.SmoothStep(InitialOffset.y, 0, _animationProgress));
			}
		}

		private void OnEnable()
		{
			_bigVirus.transform.position = InitialOffset;
			_animationProgress = 0;
		}
	}
}