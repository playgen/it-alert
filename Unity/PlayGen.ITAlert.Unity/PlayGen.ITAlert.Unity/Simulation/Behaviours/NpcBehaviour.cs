using PlayGen.ITAlert.Simulation.Common;
using PlayGen.ITAlert.Simulation.Components.Malware;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Simulation.Behaviours
{
	public class NpcBehaviour : ActorBehaviour
	{
		//private Image _timerImage;

		///private float _imageFillTimer;
		private bool _pulseDown = true;

		//private string _warningText;

		private SpriteRenderer _spriteRenderer;

		private MalwareGenome _malwareGenome;

		#region Initialization

		public void Start()
		{
			gameObject.transform.SetParent(Director.Graph.transform, false);

		}

		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			if (Entity.TryGetComponent(out _malwareGenome))
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
			//var visible = _malwareGenome.Values.Contains(SimulationConstants.MalwareVisibilityGene);
			//if (_spriteRenderer.enabled == false && visible)
			//{
			//	_spriteRenderer.enabled = true;
			//}
			//else if (_spriteRenderer.enabled && visible == false)
			//{
			//	_spriteRenderer.enabled = false;
			//}
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
		//_spriteRenderer.enabled = EntityState.Visible;

		//if (EntityState.Active)
		//{
		//	if (_pulseDown)
		//	{
		//		_spriteRenderer.color -= new Color(0, 0, 0, 0.05f);
		//	}
		//	else
		//	{
		//		_spriteRenderer.color += new Color(0, 0, 0, 0.05f);
		//	}
		//	if (_spriteRenderer.color.a <= 0)
		//	{
		//		_pulseDown = false;
		//	}
		//	else if (_spriteRenderer.color.a >= 1)
		//	{
		//		_pulseDown = true;
		//	}
		//}
		//else
		//{
		//	_pulseDown = false;
		//	_spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1);
		//}
		}

		#endregion

	}
}