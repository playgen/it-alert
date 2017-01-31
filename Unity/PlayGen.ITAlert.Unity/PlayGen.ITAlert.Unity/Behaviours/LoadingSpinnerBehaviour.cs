using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PlayGen.ITAlert.Unity.Utilities;

using UnityEngine;

namespace PlayGen.ITAlert.Unity.Behaviours
{
	public class LoadingSpinnerBehaviour : MonoBehaviour
	{
		private GameObject _panel;
		private GameObject _spinner;
		[SerializeField]
		private float _spinSpeed = 1;
		[SerializeField]
		private bool _spinClockwise = true;
		private bool _spin;

		void Awake()
		{
			LoadingUtility.LoadingSpinner = this;
			_panel = transform.Find("LoadingPanelContainer").gameObject;
			_spinner = transform.Find("LoadingPanelContainer/LoadingSpinner").gameObject;
		}

		private void FixedUpdate()
		{
			if (_spin)
			{
				_spinner.transform.Rotate(0, 0, (_spinClockwise ? -1 : 1) * _spinSpeed);
			}
		}

		public void SetSpinner(bool clockwise, float speed)
		{
			_spinClockwise = clockwise;
			_spinSpeed = speed;
		}

		public void StartSpinner()
		{
			Debug.Log("Show spinner");
			_panel.SetActive(true);
			_spinner.transform.localEulerAngles = Vector2.zero;
			_spin = true;
		}

		public void StopSpinner()
		{
			Debug.Log("Stop spinner");
			_panel.SetActive(false);
			_spin = false;
		}
	}
}
