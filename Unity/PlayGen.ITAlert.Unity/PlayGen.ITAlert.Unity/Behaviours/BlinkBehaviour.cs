using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class BlinkBehaviour : MonoBehaviour
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		private float _interval = 1f;

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
			_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1.0f);
		}

		public void Pulse(float step)
		{
			if (_pulseDown)
			{
				_image.color -= new Color(0, 0, 0, step);
			}
			else
			{
				_image.color += new Color(0, 0, 0, step);
			}
			if (_image.color.a <= 0)
			{
				_pulseDown = false;
			}
			else if (_image.color.a >= 1)
			{
				_pulseDown = true;
			}
		}

	}
}
