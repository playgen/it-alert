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

		private float _progress;

		private float _initialAlpha;

		[SerializeField]
		private AnimationCurve _curve;

		public void Awake()
		{
			_initialAlpha = _image.color.a;
		}

		public void Update()
		{
			_progress += Time.deltaTime;
			_progress %= _interval;

			_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _curve.Evaluate(_progress / _interval) * _initialAlpha);
		}

		public void OnEnable()
		{
			_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _initialAlpha);
		}

		public void OnDisable()
		{
			_image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _initialAlpha);
		}
	}
}
