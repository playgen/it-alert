using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Debugging.Scripts
{
	public class NetworkDebugMenu : MonoBehaviour
	{
		public bool AutoTickOn { get; set; }

		public bool EnableSerializer { get; set; }

		private float _elapsedInterval;

		private float _lastTickTime;

		private float _autoTickInterval = 1f/20;

		private bool _initialized;

		private string _tpsText;

		private void OnGUI()
		{
			if (GUILayout.Button("Debug Initialize"))
			{
				if (!_initialized)
				{
					Director.DebugInitialize();
					_initialized = true;
				}
			}
			EnableSerializer = GUILayout.Toggle(EnableSerializer, "Enable Serializer");
			AutoTickOn = GUILayout.Toggle(AutoTickOn, "Auto Tick ON");
			if (GUILayout.Button("TICK"))
			{
				DebugTick();
			}
			if (GUILayout.Button("Spawn Virus"))
			{
				DebugCommands.SpawnVirus();
			}
			GUILayout.Label(_tpsText);

		}

		// Update is called once per frame
		void Update()
		{
			if (AutoTickOn)
			{
				DebugAutoTick();
			}
		}

		void FixedUpdate()
		{
		}

		private void OnAutoTickToggled(bool isOn)
		{
			if (isOn == false)
			{
				_elapsedInterval = 0;
			}
		}

		public void DebugAutoTick()
		{
			_elapsedInterval += Time.deltaTime;
			if (_elapsedInterval > _autoTickInterval)
			{
				DebugTick();
			}
		}

		public void DebugTick()
		{
			_elapsedInterval = 0;
			Director.Tick(EnableSerializer);
			_tpsText = (1 / (Time.time - _lastTickTime)).ToString("N1") + "TPS";
			_lastTickTime = Time.time;
		}

		private void SpawnVirus()
		{
			DebugCommands.SpawnVirus();
		}
	}

}
