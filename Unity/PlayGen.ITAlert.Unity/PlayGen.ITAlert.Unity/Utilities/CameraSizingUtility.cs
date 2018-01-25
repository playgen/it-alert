using UnityEngine;

namespace PlayGen.ITAlert.Unity.Utilities
{
	public class CameraSizingUtility : MonoBehaviour
	{
		public float BaseAspectRatio = 16f/9f;
		public Rect GameBounds;

		private float _prevAspect;

		private Camera _cam;

		private void Awake()
		{
			_cam = GetComponent<Camera>();
			_prevAspect = BaseAspectRatio;
		}

		private void Update()
		{
			var currentAspect = (float)Screen.width / Screen.height;
			if (!Mathf.Approximately(currentAspect, _prevAspect))
			{
				_prevAspect = currentAspect;
				if (GameBounds.height * currentAspect > GameBounds.width)
				{
					_cam.orthographicSize = GameBounds.height / currentAspect;
				}
				else
				{
					_cam.orthographicSize = GameBounds.width / (currentAspect * 2);
				}
				transform.position = new Vector2(GameBounds.x, GameBounds.y + (_cam.orthographicSize - (GameBounds.height * 0.5f)));
			}
		}
	}
}
