using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Malware;
using PlayGen.ITAlert.Unity.Exceptions;
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

		[SerializeField]
		private TrailRenderer _trailRenderer;

		[SerializeField]
		private Light _light;

		private MalwareVisibility _malwareVisibility;
		private MalwareGenome _malwareGenome;

		#region Initialization
		
		protected override void OnInitialize()
		{
			base.OnInitialize();
			if (Entity.TryGetComponent(out _malwareVisibility)
				&& Entity.TryGetComponent(out _malwareGenome))
			{
				SetColor();
			}
			else
			{
				throw new SimulationIntegrationException($"Failed to load NPC component(s) for on entity {Entity.Id}");
			}
		}

		#endregion

		#region Unity Update

		protected override void OnFixedUpdate()
		{
		}

		protected override void OnUpdate()
		{
			var visible = _malwareVisibility.VisibleTo.Contains(Director.Player?.Id ?? -1);
			if (_image.enabled == false && visible)
			{
				_image.enabled = true;
			}
			else if (_image.enabled && visible == false)
			{
				_image.enabled = false;
			}
		}

		private void SetColor()
		{
			_image.color = _malwareGenome.GetColourForGenome();
			_trailRenderer.startColor = _malwareGenome.GetColourForGenome();
			_trailRenderer.endColor = new Color(_trailRenderer.startColor.r, _trailRenderer.startColor.g, _trailRenderer.startColor.b, 0.375f);
			_light.color = _malwareGenome.GetColourForGenome();
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