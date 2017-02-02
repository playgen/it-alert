using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	// ReSharper disable once CheckNamespace
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

		private float _animationProgress = 0;

		// Use this for initialization
		void Start()
		{
			Director.GameOverBehaviours.Add(_condition, this.transform.gameObject);
		}

		void Awake()
		{

		}

		// Update is called once per frame
		void Update () {
			if (_animationProgress < 1)
			{
				_animationProgress += Time.deltaTime;
				//Debug.Log(_animationProgress);
				_bigVirus.transform.localPosition = new Vector3(InitialOffset.x, Mathf.SmoothStep(InitialOffset.y, 0, _animationProgress));
			}
		}

		void OnEnable()
		{
			_bigVirus.transform.position = InitialOffset;
			_animationProgress = 0;
		}
	}
}