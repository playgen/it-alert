using PlayGen.ITAlert.Simulation.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation
{
	public class AnimateConnectionUV : MonoBehaviour
	{
		[SerializeField]
		public Vector2 DefaultAnimationRate = new Vector2(1.0f, 0.0f);

		[SerializeField]
		public Vector2 AnimationRate;

		[SerializeField]
		private Image _image;

		Vector2 uvOffset = Vector2.zero;

		void Awake()
		{
			AnimationRate = DefaultAnimationRate;

			var material = Instantiate(_image.material);
			_image.material = material;
		}

		void LateUpdate()
		{
			uvOffset += ((float)SimulationConstants.PlayerSpeed *  AnimationRate * Time.deltaTime);
			if (_image.enabled)
			{
				_image.materialForRendering.mainTextureOffset = uvOffset;
			}
		}
	}
}
