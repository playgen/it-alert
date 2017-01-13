using UnityEngine;

namespace PlayGen.ITAlert.Unity.Settings
{
	public class SliderHandleSizeSetter : MonoBehaviour
	{
		private void Update()
		{
			if (transform.hasChanged)
			{
				((RectTransform) transform).sizeDelta = new Vector2(((RectTransform) transform.parent).rect.height * 0.6f,
					((RectTransform) transform).sizeDelta.y);
			}
		}
	}
}