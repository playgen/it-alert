using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class BlinkBehaviour : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;

		[SerializeField] private float _interval = 1f;

		private float _step = 0.05f;

		private bool _pulseDown;

		public void AWake()
		{
		}

		public void Update()
		{
			Pulse(Time.deltaTime / _interval / 2);
		}

		public void OnEnable()
		{
			_pulseDown = true;
		}

		public void OnDisable()
		{
			_spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1.0f);
		}

		public void Pulse(float step)
		{
			if (_pulseDown)
			{
				_spriteRenderer.color -= new Color(0, 0, 0, step);
			}
			else
			{
				_spriteRenderer.color += new Color(0, 0, 0, step);
			}
			if (_spriteRenderer.color.a <= 0)
			{
				_pulseDown = false;
			}
			else if (_spriteRenderer.color.a >= 1)
			{
				_pulseDown = true;
			}
		}

	}
}
