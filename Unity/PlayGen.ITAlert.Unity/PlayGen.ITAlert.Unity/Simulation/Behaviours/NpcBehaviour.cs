using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Malware;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class NpcBehaviour : ActorBehaviour
	{
		//private Image _timerImage;

		///private float _imageFillTimer;
		private bool _pulseDown = true;

		//private string _warningText;

		[SerializeField]
		private Image _image;

		private MalwareVisibility _malwareVisibility;

		#region Initialization


		protected override void OnInitialize()
		{
			base.OnInitialize();
			if (Entity.TryGetComponent(out _malwareVisibility))
			{
				
			}
		}

		#endregion

		#region Unity Update

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
			var visible = _malwareVisibility.Visible;
			if (_image.enabled == false && visible)
			{
				_image.enabled = true;
			}
			else if (_image.enabled && visible == false)
			{
				_image.enabled = false;
			}
		}

		#endregion

		#region State Update

		protected override void OnStateUpdated()
		{
			HandlePulse();
			UpdatePosition();
		}

		private void HandlePulse()
		{
		//TODO: reimplement pulse => visible property
		//_image.enabled = EntityState.Visible;

		//if (EntityState.Active)
		//{
		//	if (_pulseDown)
		//	{
		//		_image.color -= new Color(0, 0, 0, 0.05f);
		//	}
		//	else
		//	{
		//		_image.color += new Color(0, 0, 0, 0.05f);
		//	}
		//	if (_image.color.a <= 0)
		//	{
		//		_pulseDown = false;
		//	}
		//	else if (_image.color.a >= 1)
		//	{
		//		_pulseDown = true;
		//	}
		//}
		//else
		//{
		//	_pulseDown = false;
		//	_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
		//}
		}

		#endregion

	}
}